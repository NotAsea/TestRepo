# PROJECT NGICH

## Stack use

- ASP.NET Core Minimal API 8
- Mapperly
- DB: postgresql
- Bogus to generate seed data
- EF core 8
- [EFCore.GenericRepository](https://github.com/TanvirArjel/EFCore.GenericRepository) *excellent project recommend
  checking out*
- [Z.EntityFramework.Extensions](https://github.com/zzzprojects/EntityFramework-Extensions)
- FluentValidation
- ~~almost anything use source gen json serializer (except swagger)~~ 
add FluentValidation and oh boy it also uses `System.Reflection`

## Attention

This project use swagger which use `System.Reflection`
under the hood, and of course `Entity Framework Core`, and `FluentValidation` which mean this project can not be **Publish AOT** or **Publish Trimmed**

Although Microsoft claims `RequestDelegateGenerator` is working smoothly, enable it cause false compile,
however they
said [it's fixed in net9.0-preview](https://github.com/dotnet/aspnetcore/issues/52989#issuecomment-1871452410)
haven't try it yet, if you interest, you can give it a try