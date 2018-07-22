namespace VEdit.Plugins
{
    /// <summary>
    /// Should be placed on functions that given the same input return the same output.
    /// </summary>
    public class PureNodeAttribute : NodeAttribute
    {
        /// <summary>
        /// Removes the title bar and puts the text in the middle of the node.
        /// Overrides DisplayName.
        /// </summary>
        public string CompactNodeTitle { get; set; }
    }
}
