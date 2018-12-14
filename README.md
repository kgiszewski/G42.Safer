# G42.Safer

## What is this?

WebApi does not protect automatically for XSS, this library is an attempt to make responses to a consuming client safer. Note that it uses the terminology of `safer` and not `safe`. Please consider this as only one-layer of protection. You should present a malicious user with several layers of defense.

## Theory of defense

When it comes to XSS protection, two thoughts should come to your mind:
1) Sanitizing input
2) Encoding sensitive characters on the out-stream

This simple library helps accomplish the latter. I have been working on some Regex data annotations that are not quite ready for prime time.

## What can you do?

```
//given a string like this that might make it into your system, this library will encode it like so with an simple extension method:

var unsafeString = "<script>alert('hello');</script><a href='#' onClick='javascript:void();'>Foo</a>";

var betterString = unsafeString.ToSaferString(); //&lt;script&gt;alert(&#39;hello&#39;);&lt;/script&gt;&lt;a href=&#39;#&#39; onClick=&#39;javascript:void();&#39;&gt;Foo&lt;/a&gt;

```

That may not be an amazing feat of coding but typically you have a service return an object with nested properties.

The next extension method will look through an object and sanitize output:

```
var someObject = _someService.GetStuff();
...
return Ok(someObject.ToSaferObject());

```

## Limitations

`.ToSaferObject()` uses reflection to iterate objects. Reflection is unable to easily iterate an indexed property (e.g. a `Dictionary<>`). If you are using indexed properties, you will have to handle the iteration of those objects your self and apply `.ToSaferString()` yourself.

## Thoughts

MVC/Razor provide some XSS protections but you won't find them in naked WebApi. Again this is just a layer of defense, please sanitize input as well.

## Nuget
https://www.nuget.org/packages/G42.Safer/

## Future

I want to provide a more semantic collection of data validation attributes for checking `ModelState`. I have several being used in the wild but want to see how they shake out first. 

For example:
- `[ZipCode]`
- `[PhoneNumber]`
- `[Address]`
- `[ProperNoun]`
- `[Url]`
- `[AlphaNumeric]`
- `[Numbers]`
- `[EmailAddress]`

Some of these already exist in the common libs but I want to create a more definitive list.

