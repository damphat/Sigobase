# Sigobase.Generator
Generate all possible sigo values using SigoSchema

This project will generate data for unit tests, making writing unit tests easier and fun.


```cs
// define schemas with syntax: <Name> = <SchemaDefinition> 
SigoSchema.Parse("money = 'usd' | 'eur' ;  number = 50|100");

// anonumous schema
Schame account = SigoSchema.Parse("{ kind: money, amount?: number }");

// generate
IEnumerable<ISigo> allPossibleValues = account.Generate();

foreach(ISigo v in allPossibleValues) {
	Console.WriteLine(v);
}
// Ouput
// {kind:'eur'}
// {kind:'usd'}
// {kind:'eur', amount:50}
// {kind:'eur', amount:100}
// {kind:'usd', amount:50}
// {kind:'usd', amount:100}

```

## Features
explains with Sigobase.Generator.REPL

### List:
```
schema> 1 | 2 | 'a' | 'b'
1
2
'a'
'b'
```

### Define and reference:
* Define a schema: ```<name> = <schemaDefinition>```.
* Allow redefine schemas 
* Use schemas to define new schemas.
* Reference to unknown schema allow at define-time, but it throw exception at generate-time.

```
schema> letter = 'a' | 'b'
schema> number = 1 | 2
schema> letter | number  // reference to letter or number
1
2
'a'
'b'
```

### Flag:
- Sigo object has 8 possible proton flags 0->7
- Default is 3
- {?} is short version of {01234567}

```
schema> {234}
{2}
{}
{4}

schema> {}  // default is 3
{}

schema> {?}  // all
{0}
{1}
{2}
{}
{4}
{5}
{6}
{7}
```

### Optional field:
```
schema> {x?:1,  y?:1}
{}
{x:1}
{y:1}
{x:1, y:1}
```

### Auto field:
```
schema> {money}   // auto convert to {money: money}
{money: 'usd'}
{money: 'eur'}

schema> {money?}   // auto and optional
{}
{money: 'usd'}
{money: 'eur'}
```

## TODO
- [ ] `xUnit` is multithread. Make `Parse()` thread safe.
- [ ] Loop detection `schema> Person = {me: Person}`
- [ ] Change `schema.Parse()` to `schema.Eval()` ?
