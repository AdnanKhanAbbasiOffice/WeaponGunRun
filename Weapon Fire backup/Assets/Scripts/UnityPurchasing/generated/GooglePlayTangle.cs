// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("lRYYFyeVFh0VlRYWF/PJPpAzCTVbriTufz3vdklufhJVAay1SBkwcL0nM3ofdBIJ7EDYUDsQhYQll+uUf651H9LYEcyDvppeBnM5S3ynaxkdwwlm96/ZIp/wVy1GS1bqGDVp2hILcgcGX6fLst2rPXCeGN1oSTe98CSrWFyUXTpe2K0gM8z0pAvDaBIhIw3nVD4IaH7/8ULmL5RkZvamk68La0R6x/v28BW4zE2cfa52EhU2lNKQLwgCTB1auDyp+IDDVQ1Q5DXwQ8+6yoihob/26wVurkaa31aFk4WdR4U/4tmBeAxnlsSZrTVgIS7OdfLav+hTkqb0YnVXamEtn25kXcgnlRY1JxoRHj2RX5HgGhYWFhIXFPa0sMkpRjEHtBUUFhcW");
        private static int[] order = new int[] { 1,2,9,5,8,7,10,11,10,13,12,12,12,13,14 };
        private static int key = 23;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
