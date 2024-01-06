# PROJECT NGICH

## Stack use

- ASP.NET Core Minimal API 8
- Mapperly
- DB: postgresql
- Bogus to generate seed data
- EF core 8
- [EFCore.GenericRepository](https://github.com/TanvirArjel/EFCore.GenericRepository) *excellent project recommend checking out*
- [Z.EntityFramework.Extensions](https://github.com/zzzprojects/EntityFramework-Extensions) 
- almost anything use source gen json serializer (except swagger)

## Attention

Although everything is Trimmed friendly, this project use swagger which use `System.Reflection`
under the hood, and of course `Entity Framework Core`, which mean this project can not be **Publish AOT**

Although Microsoft claims `RequestDelegateGenerator` is working smoothly, enable it cause false compile,
however they said [it's fixed in net9.0-preview](https://github.com/dotnet/aspnetcore/issues/52989#issuecomment-1871452410)
havent try it yet, if you interest, you can give it a try