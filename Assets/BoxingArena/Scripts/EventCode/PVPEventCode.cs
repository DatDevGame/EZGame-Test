/// <summary>
/// Defines the event codes used in PvP gameplay events.
/// </summary>
public enum PVPEventCode
{
    /// <summary>
    /// Triggered when a character receives damage in PvP mode.
    ///
    /// <para>Parameters:</para>
    /// <list type="number">
    ///   <item>
    ///     <description><c>0</c> — <see cref="BaseBoxer"/>: The character that received the damage.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>1</c> — <see cref="int"/>: The amount of damage received.</description>
    ///   </item>
    /// </list>
    /// </summary>    
    CharacterReceivedDamage
}
