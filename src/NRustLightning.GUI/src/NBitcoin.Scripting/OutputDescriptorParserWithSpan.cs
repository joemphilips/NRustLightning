namespace NBitcoin.Scripting
{
    #if HAS_SPAN
    public static class OutputDescriptorParserWithSpan
    {
        private static PubkeyProvider ParsePubkeyInner(uint32 keyExpInde, Span<sp> sp) {}
    }
    #endif
}