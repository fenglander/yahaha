using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Furion.IntegrationTests;

/// <summary>
/// ��ͼ���漯�ɲ���
/// </summary>
public class ViewEngineTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ITestOutputHelper Output;
    private readonly WebApplicationFactory<Program> _factory;

    public ViewEngineTests(ITestOutputHelper tempOutput,
        WebApplicationFactory<Program> factory)
    {
        Output = tempOutput;
        _factory = factory;
    }

    /// <summary>
    /// ���� RunCompile �� RunCompileFromCached ������������ģ�ͣ�
    /// </summary>
    /// <param name="url"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ViewEngineTests/TestRunCompile", "Furion")]
    [InlineData("/ViewEngineTests/TestRunCompileFromCached", "Furion")]
    public async Task Test_RunCompile_Or_FromCached(string url, string name)
    {
        using var httpClient = _factory.CreateClient();
        using var response = await httpClient.PostAsync($"{url}/{name}", default);

        var content = await response.Content.ReadAsStringAsync();
        Output.WriteLine(content);
        response.EnsureSuccessStatusCode();

        Assert.Equal($"Hello {name}", content);
    }

    /// <summary>
    /// ���� RunCompile �� RunCompileFromCached ������ǿ����ģ�ͣ�
    /// </summary>
    /// <param name="url"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ViewEngineTests/TestRunCompileStrongly", "Furion", 1, 2, 3)]
    [InlineData("/ViewEngineTests/TestRunCompileStronglyFromCached", "Furion", 1, 2, 3)]
    public async Task Test_RunCompile_Strongly_Or_FromCached(string url, string name, params int[] items)
    {
        var model = new { Name = name, Items = items };

        using var httpClient = _factory.CreateClient();
        using var response = await httpClient.PostAsync($"{url}", new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"));

        var content = await response.Content.ReadAsStringAsync();
        Output.WriteLine(content);
        response.EnsureSuccessStatusCode();

        Assert.Equal($@"Hello Furion
    <p>1</p>
    <p>2</p>
    <p>3</p>
", content);
    }

    /// <summary>
    /// ���� RunCompile ��ӳ��򼯻���������ռ�
    /// </summary>
    /// <param name="url"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ViewEngineTests/TestRunCompileAssembly", "Furion")]
    [InlineData("/ViewEngineTests/TestRunCompileAddAssemblyAndNamespace", "Furion")]
    public async Task Test_RunCompile_AddAssembly_Or_AddNamespace(string url, string name)
    {
        using var httpClient = _factory.CreateClient();
        using var response = await httpClient.PostAsync($"{url}/{name}", default);

        var content = await response.Content.ReadAsStringAsync();
        Output.WriteLine(content);
        response.EnsureSuccessStatusCode();

        Assert.Equal($@"<div>{name}\ViewEngine</div>", content);
    }

    /// <summary>
    /// ���� RunCompile ����ģ�巽��
    /// </summary>
    /// <param name="url"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ViewEngineTests/TestRunCompileIncludeFunctionWithInvoke")]
    public async Task Test_RunCompile_IncludeFunction_With_Invoke(string url)
    {
        using var httpClient = _factory.CreateClient();
        using var response = await httpClient.PostAsync(url, default);

        var content = await response.Content.ReadAsStringAsync();
        Output.WriteLine(content);
        response.EnsureSuccessStatusCode();

        Assert.Equal($@"<area>
    <div>LEVEL: 3</div>
    <div>LEVEL: 2</div>
    <div>LEVEL: 1</div>
</area>

", content);
    }

    /// <summary>
    /// ���� RunCompile ��ӳ��򼯻���������ռ�
    /// </summary>
    /// <param name="url"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("/ViewEngineTests/TestRunCompileStronglyInvokeClassMethod")]
    public async Task Test_RunCompile_Strongly_Invoke_ClassMethod(string url)
    {
        using var httpClient = _factory.CreateClient();
        using var response = await httpClient.PostAsync(url, default);

        var content = await response.Content.ReadAsStringAsync();
        Output.WriteLine(content);
        response.EnsureSuccessStatusCode();

        Assert.Equal($@"Hello 10, Furion, -=123=-", content);
    }
}