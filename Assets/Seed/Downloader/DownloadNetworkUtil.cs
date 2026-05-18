using UnityEngine;

namespace Seed.Downloader
{
    // 다운로드 사이즈 단위
    public enum SizeUnits
    {
        Byte, KB, MB, GB
    }

    public static class DownloadNetworkUtil
    {
        // 네트워크가 연결되어 있는가?
        public static bool IsNetworkValid()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        // 현재 Cache 에 requiredSize 이상의 여유 공간이 있는가?
        public static bool IsDiskSpaceEnough(long requiredSize)
        {
            return Caching.defaultCache.spaceFree >= requiredSize;
        }

        #region ====:: Size 관련 ::====

        public const long ONE_GB = 1_000_000_000;
        public const long ONE_MB = 1_000_000;
        public const long ONE_KB = 1_000;

        // 바이트 byteSize 사이즈에 맞게끔 적절한 단위 SizeUnits 타입을 가져온다
        public static SizeUnits GetProperByteUnit(long byteSize)
        {
            if (byteSize >= ONE_GB) return SizeUnits.GB;
            if (byteSize >= ONE_MB) return SizeUnits.MB;
            if (byteSize >= ONE_KB) return SizeUnits.KB;
            return SizeUnits.Byte;
        }

        // 바이트를 byteSize, unit 단위에 맞게 숫자를 변환한다
        public static long ConvertByteByUnit(long byteSize, SizeUnits unit)
        {
            return (long)(byteSize / System.Math.Pow(1024, (long)unit));
        }

        // 바이트를 byteSize 단위와 함께 출력이 가능한 문자열 형태로 변환한다
        public static string GetConvertedByteString(long byteSize, SizeUnits unit, bool appendUnit = true)
        {
            string unitStr = appendUnit ? unit.ToString() : string.Empty;
            return $"{ConvertByteByUnit(byteSize, unit):0.00}{unitStr}";
        }

        #endregion
    }
}
