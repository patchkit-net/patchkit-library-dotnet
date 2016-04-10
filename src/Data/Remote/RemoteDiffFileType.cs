namespace PatchKit.Data.Remote
{
    /// <summary>
    /// Type of file included in diff.
    /// </summary>
    internal enum RemoteDiffFileType
    {
        /// <summary>
        /// File has been added. Must be copied into local data.
        /// </summary>
        Added,
        /// <summary>
        /// File has been added. Local file must be patched.
        /// </summary>
        Modified,
        /// <summary>
        /// File has been removed. Local file must be removed.
        /// </summary>
        Removed
    }
}
