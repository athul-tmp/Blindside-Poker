namespace Blindside.Engine.Settings;

// One problem with one setting. Field is the GameSettings property name,
// so the host's form can show the message next to the right input.
public record ValidationError(string Field, string Message);
