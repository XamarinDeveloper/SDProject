namespace SDProject.Extensions {
    public static class CharExtension {
        public static int ToInt32(this char ch) {
            return ch.ToString().ToInt32();
        }
    }
}