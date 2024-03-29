// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Furion.SpecificationDocument;
using Lazy.Captcha.Core;
using Yahaha.Core.Ldap;
using Yahaha.Core.Models;
using Yahaha.Core.Service.Role.Dto;

namespace Yahaha.Core.Service;

/// <summary>
/// 系统登录授权服务
/// </summary>
[ApiDescriptionSettings(Order = 500)]
public class SysAuthService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SysMenuService _sysMenuService;
    private readonly SysOnlineUserService _sysOnlineUserService;
    private readonly SysConfigService _sysConfigService;
    private readonly ICaptcha _captcha;
    private readonly SysCacheService _sysCacheService;
    private readonly LdapService _ldapService;
    private readonly ISqlSugarClient _db;

    public SysAuthService(UserManager userManager,
        SqlSugarRepository<SysUser> sysUserRep,
        IHttpContextAccessor httpContextAccessor,
        SysMenuService sysMenuService,
        SysOnlineUserService sysOnlineUserService,
        SysConfigService sysConfigService,
        ICaptcha captcha,
        ISqlSugarClient db,
        SysCacheService sysCacheService,
        LdapService ldapService)
    {
        _userManager = userManager;
        _sysUserRep = sysUserRep;
        _httpContextAccessor = httpContextAccessor;
        _sysMenuService = sysMenuService;
        _sysOnlineUserService = sysOnlineUserService;
        _sysConfigService = sysConfigService;
        _captcha = captcha;
        _sysCacheService = sysCacheService;
        _ldapService = ldapService;
        _db = db;
    }

    /// <summary>
    /// 登录系统
    /// </summary>
    /// <param name="input"></param>
    /// <remarks>用户名/密码：superadmin/123456</remarks>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("登录系统")]
    public async Task<LoginOutput> Login([Required] LoginInput input)
    {
        DataElement DataElement = new DataElement(_db);
        //// 可以根据域名获取具体租户
        //var host = _httpContextAccessor.HttpContext.Request.Host;

        // 是否开启验证码
        if (await _sysConfigService.GetConfigValue<bool>(CommonConst.SysCaptcha))
        {
            // 判断验证码
            if (!_captcha.Validate(input.CodeId.ToString(), input.Code))
                throw Oops.Oh(ErrorCodeEnum.D0008);
        }

        // 账号是否存在
        var Row = DataElement.Search("SysUser").Where("Account = @account", new { account = input.Account }).ToList();
        DrillDownDataDto DrillDownParams = new DrillDownDataDto
        {
            model = "SysUser",
            items = Row,
        };
        dynamic user = DataElement.DrillDownData(DrillDownParams).FirstOrDefault();

        //var user = await _sysUserRep.AsQueryable().Includes(t => t.SysOrg).Filter(null, true).FirstAsync(u => u.Account.Equals(input.Account));
        _ = user ?? throw Oops.Oh(ErrorCodeEnum.D0009);

        // 账号是否被冻结
        StatusEnum UserStatus = (StatusEnum)Enum.Parse(typeof(StatusEnum), user.Status.ToString());
        if (UserStatus == StatusEnum.Disable)
            throw Oops.Oh(ErrorCodeEnum.D1017);

        // 租户是否被禁用
        long UserTenantId = user.TenantId;
        var tenant = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysTenant>>().GetFirstAsync(u => u.Id == UserTenantId);
        if (tenant != null && tenant.Status == StatusEnum.Disable)
            throw Oops.Oh(ErrorCodeEnum.Z1003);

        // 域账号是否能登录
        var isLdapCheck = _ldapService.Validate(input.Account, input.Password, tenant);
        if (!isLdapCheck)
        {
            // 密码是否正确
            if (CryptogramUtil.CryptoType == CryptogramEnum.MD5.ToString())
            {
                if (user.Password != MD5Encryption.Encrypt(input.Password))
                    throw Oops.Oh(ErrorCodeEnum.D1000);
            }
            else
            {
                if (CryptogramUtil.Decrypt(user.Password) != input.Password)
                    throw Oops.Oh(ErrorCodeEnum.D1000);
            }
        }
        else
        {
            user.Password = CryptogramUtil.Encrypt(input.Password);
            _sysUserRep.Update(user);
        }

        // 单用户登录
        await _sysOnlineUserService.SignleLogin(user.Id);

        var tokenExpire = await _sysConfigService.GetTokenExpire();
        var refreshTokenExpire = await _sysConfigService.GetRefreshTokenExpire();

        // 生成Token令牌
        var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
        {
            { ClaimConst.UserId, user.Id },
            { ClaimConst.TenantId, user.TenantId },
            { ClaimConst.Account, user.Account },
            { ClaimConst.RealName, user.RealName },
            { ClaimConst.AccountType, user.AccountType },
            { ClaimConst.OrgId, user.OrgId },
            { ClaimConst.OrgName, user.SysOrg?.Name ?? "" },   // 如果 user.SysOrg 为 null，则设为默认值
            { ClaimConst.OrgType, user.SysOrg?.OrgType ?? "" }, // 如果 user.SysOrg 为 null，则设为默认值
        }, tokenExpire);

        // 生成刷新Token令牌
        var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, refreshTokenExpire);

        // 设置响应报文头
        _httpContextAccessor.HttpContext.SetTokensOfResponseHeaders(accessToken, refreshToken);

        // Swagger Knife4UI-AfterScript登录脚本
        // ke.global.setAllHeader('Authorization', 'Bearer ' + ke.response.headers['access-token']);

        return new LoginOutput
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    /// <summary>
    /// 获取登录账号
    /// </summary>
    /// <returns></returns>
    [DisplayName("登录系统")]
    public async Task<LoginUserOutput> GetUserInfo()
    {
        var user = await _sysUserRep.GetFirstAsync(u => u.Id == _userManager.UserId);
        if (user == null)
            throw Oops.Oh(ErrorCodeEnum.D1011).StatusCode(401);

        // 获取机构
        var org = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysOrg>>().GetFirstAsync(u => u.Id == user.OrgId);
        // 获取职位
        var pos = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysPos>>().GetFirstAsync(u => u.Id == user.PosId);
        // 获取拥有按钮权限集合
        var buttons = await _sysMenuService.GetOwnBtnPermList();

        return new LoginUserOutput
        {
            Id = user.Id,
            Account = user.Account,
            RealName = user.RealName,
            Avatar = user.Avatar,
            Address = user.Address,
            Signature = user.Signature,
            OrgId = user.OrgId,
            OrgName = org?.Name,
            OrgType = org?.OrgType,
            PosName = pos?.Name,
            Buttons = buttons
        };
    }

    /// <summary>
    /// 获取刷新Token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    [DisplayName("获取刷新Token")]
    public string GetRefreshToken([FromQuery] string accessToken)
    {
        var refreshTokenExpire = _sysConfigService.GetRefreshTokenExpire().GetAwaiter().GetResult();
        return JWTEncryption.GenerateRefreshToken(accessToken, refreshTokenExpire);
    }

    /// <summary>
    /// 退出系统
    /// </summary>
    [DisplayName("退出系统")]
    public void Logout()
    {
        if (string.IsNullOrWhiteSpace(_userManager.Account))
            throw Oops.Oh(ErrorCodeEnum.D1011);

        _httpContextAccessor.HttpContext.SignoutToSwagger();
    }

    /// <summary>
    /// 获取登录配置
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [SuppressMonitor]
    [DisplayName("获取登录配置")]
    public async Task<dynamic> GetLoginConfig()
    {
        var secondVerEnabled = await _sysConfigService.GetConfigValue<bool>(CommonConst.SysSecondVer);
        var captchaEnabled = await _sysConfigService.GetConfigValue<bool>(CommonConst.SysCaptcha);
        return new { SecondVerEnabled = secondVerEnabled, CaptchaEnabled = captchaEnabled };
    }

    /// <summary>
    /// 获取水印配置
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("获取水印配置")]
    public async Task<dynamic> GetWatermarkConfig()
    {
        var watermarkEnabled = await _sysConfigService.GetConfigValue<bool>(CommonConst.SysWatermark);
        return new { WatermarkEnabled = watermarkEnabled };
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [SuppressMonitor]
    [DisplayName("获取验证码")]
    public dynamic GetCaptcha()
    {
        var codeId = YitIdHelper.NextId().ToString();
        var captcha = _captcha.Generate(codeId);
        return new { Id = codeId, Img = captcha.Base64 };
    }

    /// <summary>
    /// Swagger登录检查
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("/swagger/checkUrl"), NonUnify]
    [DisplayName("Swagger登录检查")]
    public int SwaggerCheckUrl()
    {
        return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ? 200 : 401;
    }

    /// <summary>
    /// Swagger登录提交
    /// </summary>
    /// <param name="auth"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("/swagger/submitUrl"), NonUnify]
    [DisplayName("Swagger登录提交")]
    public async Task<int> SwaggerSubmitUrl([FromForm] SpecificationAuth auth)
    {
        try
        {
            _sysCacheService.Set(CommonConst.SysCaptcha, false);

            await Login(new LoginInput
            {
                Account = auth.UserName,
                Password = auth.Password
            });

            _sysCacheService.Remove(CommonConst.SysCaptcha);

            return 200;
        }
        catch (Exception)
        {
            return 401;
        }
    }
}