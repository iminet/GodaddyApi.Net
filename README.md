# GodaddyApi.Net
This is a simple API solution for handling your GoDaddy DNS records

## Requirements
- .NET 4.5 or :NET Core 3.1
- Newtonsoft Json.NET from [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/)
- Developer key and secret from [GoDaddy](https://developer.godaddy.com/keys)


## Usage

### This is a basic sample code for getting current DNS record

```
Iminetsoft.Godaddy gd = new Iminetsoft.Godaddy();
gd.Domain = "yourdomain.com";
gd.Type = Iminetsoft.Godaddy.RecordTypes.A;
gd.Name = "yourrecordname" // (for example @ for the domain itself or test for a subdomain)";
gd.Ttl = 600;
gd.Secret = "YourSecret";
gd.Key = "YourKey";
gd.GetDnsRecord();
Console.WriteLine(gd.Data);
```

### And now I going to show you, how can you update a DNS record (or create if it didn't exist before)
```
Iminetsoft.Godaddy gd = new Iminetsoft.Godaddy();
gd.Domain = "yourdomain.com";
gd.Type = Iminetsoft.Godaddy.RecordTypes.A;
gd.Name = "yourrecordname" // (for example @ for the domain itself or test for a subdomain)";
gd.Ttl = 600;
gd.Secret = "YourSecret";
gd.Key = "YourKey";
gd.Data = "8.8.8.8" // Here goes your new IP address
gd.SetDnsRecord();
```

## Other info
Please keep in your mind, this code is still under development, so I am not responsible for it.
Let me know if you have any issue or idea how should I make it better.
