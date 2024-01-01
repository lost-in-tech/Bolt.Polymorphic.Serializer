using Bolt.Common.Extensions;

namespace Bolt.Polymorphic.Serializer.Tests.Helpers;

public static class ShouldlyExtensions
{
    public static void ShouldMatchApprovedDefault<T>(this T source, string? msg = null, string? discriminator = null)
    {
        ShouldMatchApprovedDefault(source.SerializeToPrettyJson(), msg, discriminator);
    }
    
    public static void ShouldMatchApprovedDefault(this string source, string? msg = null, string? discriminator = null)
    {
        source.ShouldMatchApproved(opt =>
        {
            opt.UseCallerLocation();
            opt.SubFolder("approvals");
            if(discriminator != null) opt.WithDiscriminator(discriminator);
        }, msg);
    }
}