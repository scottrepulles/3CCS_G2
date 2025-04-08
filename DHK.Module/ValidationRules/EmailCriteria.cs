using DevExpress.Persistent.Validation;
using DHK.Module.Constants;
using DKH.Module.Constants;

namespace DHK.Module.ValidationRules;

public static class EmailCriteria
{
    public const DefaultContexts Context = DefaultContexts.Save;
    public const string Pattern = Patterns.EMAIL_REGEX;
    public const string Message = CustomMessages.INVALID_EMAIL;
}
