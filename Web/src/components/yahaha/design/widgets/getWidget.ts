// getWidget.ts
 
const gets = {} as any 
const modules = import.meta.glob('./*.vue', {eager:true})
for (let each in modules) {
  const name = (modules[each] as any).default.__name
  gets[name] = (modules[each] as any).default
}
 
 
export default gets