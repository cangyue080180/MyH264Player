using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyH264Player
{
    /// <summary>
    /// haisiH264解码类
    /// </summary>
    public class H264Dec
    {
        public const int HI_SUCCESS = 0;

        public const int HI_FAILURE = -1;

        public const int HI_LITTLE_ENDIAN = 1234;

        public const int HI_BIG_ENDIAN = 4321;

        public const int HI_DECODER_SLEEP_TIME = 60000;

        public const int HI_H264DEC_OK = 0;

        public const int HI_H264DEC_NEED_MORE_BITS = -1;

        public const int HI_H264DEC_NO_PICTURE = -2;

        public const int HI_H264DEC_ERR_HANDLE = -3;

        //创建，初始化解码器句柄
        [DllImport("hi_h264dec_w.dll", EntryPoint = "Hi264DecCreate", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Hi264DecCreate(ref hiH264_DEC_ATTR_S pDecAttr);
        //销毁解码器句柄
        [DllImport("hi_h264dec_w.dll", EntryPoint = "Hi264DecDestroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Hi264DecDestroy(IntPtr hDec);

        //查询解码库版本信息和当前版本能力集
        [DllImport("hi_h264dec_w.dll", EntryPoint = "Hi264DecGetInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hi264DecGetInfo(ref hiH264_LIBINFO_S pLibInfo);

        /// <summary>
        /// 对输入的一段码流进行解码并按帧输出图像
        /// </summary>
        /// <param name="hDec">解码器句柄</param>
        /// <param name="pStream">码流起始地址</param>
        /// <param name="iStreamLen">码流长度</param>
        /// <param name="ullPTS">时间戳信息</param>
        /// <param name="pDecFrame">图像信息</param>
        /// <param name="uFlags">解码模式 0：正常解码；1、解码完毕并要求解码器输出残留图像</param>
        /// <returns></returns>
        [DllImport("hi_h264dec_w.dll", EntryPoint = "Hi264DecFrame", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hi264DecFrame(IntPtr hDec, IntPtr pStream, uint iStreamLen, ulong ullPTS, ref hiH264_DEC_FRAME_S pDecFrame, uint uFlags);
        //对输入的一帧图像对应的码流进行解码并立即输出此帧图像
        [DllImport("hi_h264dec_w.dll", EntryPoint = "Hi264DecAU", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hi264DecAU(IntPtr hDec, IntPtr pStream, uint iStreamLen, ulong ullPTS, ref hiH264_DEC_FRAME_S pDecFrame, uint uFlags);
        //解码后图像增强
        [DllImport("hi_h264dec_w.dll", EntryPoint = "Hi264DecImageEnhance", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hi264DecImageEnhance(IntPtr hDec, ref hiH264_DEC_FRAME_S pDecFrame, uint uEnhanceCoeff);

    }
    /// <summary>
    /// 解码器属性信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct hiH264_DEC_ATTR_S
    {
        /// <summary>
        /// 解码器输出图像格式，目前解码库只支持YUV420图像格式
        /// </summary>
        public uint uPictureFormat;
        /// <summary>
        /// 输入码流格式 0x00: 目前解码库只支持以“00 00 01”为nalu分割符的流式H.264码流 
        /// </summary>
        public uint uStreamInType;
        /// <summary>
        /// 图像宽度
        /// </summary>
        public uint uPicWidthInMB;
        /// <summary>
        /// 图像高度
        /// </summary>
        public uint uPicHeightInMB;
        /// <summary>
        /// 参考帧数目
        /// </summary>
        public uint uBufNum;
        /// <summary>
        /// 解码器工作模式
        /// </summary>
        public uint uWorkMode;
        /// <summary>
        /// 用户私有数据
        /// </summary>
        public IntPtr pUserData;
        /// <summary>
        /// 保留字
        /// </summary>
        public uint uReserved;

    }

    /// <summary>
    /// 解码器输出图像信息数据结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct hiH264_DEC_FRAME_S
    {
        /// <summary>
        /// Y分量地址
        /// </summary>
        public IntPtr pY;
        /// <summary>
        /// U分量地址
        /// </summary>
        public IntPtr pU;
        /// <summary>
        /// V分量地址
        /// </summary>
        public IntPtr pV;
        /// <summary>
        /// 图像宽度(以像素为单位)
        /// </summary>
        public uint uWidth;
        /// <summary>
        /// 图像高度(以像素为单位)
        /// </summary>
        public uint uHeight;
        /// <summary>
        /// 输出Y分量的stride (以像素为单位)
        /// </summary>
        public uint uYStride;
        /// <summary>
        /// 输出UV分量的stride (以像素为单位)
        /// </summary>
        public uint uUVStride;
        /// <summary>
        /// 图像裁减信息:左边界裁减像素数
        /// </summary>
        public uint uCroppingLeftOffset;
        /// <summary>
        /// 图像裁减信息:右边界裁减像素数
        /// </summary>
        public uint uCroppingRightOffset;
        /// <summary>
        /// 图像裁减信息:上边界裁减像素数
        /// </summary>
        public uint uCroppingTopOffset;
        /// <summary>
        /// 图像裁减信息:下边界裁减像素数
        /// </summary>
        public uint uCroppingBottomOffset;
        /// <summary>
        /// 输出图像在dpb中的序号
        /// </summary>
        public uint uDpbIdx;
        /// <summary>
        /// 图像类型：0:帧; 1:顶场; 2:底场 */
        /// </summary>
        public uint uPicFlag;
        /// <summary>
        /// 图像类型：0:帧; 1:顶场; 2:底场 */
        /// </summary>
        public uint bError;
        /// <summary>
        /// 图像是否为IDR帧：0:非IDR帧;1:IDR帧
        /// </summary>
        public uint bIntra;
        /// <summary>
        /// 时间戳
        /// </summary>
        public ulong ullPTS;
        /// <summary>
        /// 图像信号
        /// </summary>
        public uint uPictureID;
        /// <summary>
        /// 保留字
        /// </summary>
        public uint uReserved;
        /// <summary>
        /// 指向用户私有数据
        /// </summary>
        public IntPtr pUserData;

    }


    /// <summary>
    /// 解码库版本、版权和能力集信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct hiH264_LIBINFO_S
    {
        /// <summary>
        /// 主编号
        /// </summary>
        public uint uMajor;
        /// <summary>
        /// 次编号
        /// </summary>
        public uint uMinor;
        /// <summary>
        /// 发布编号
        /// </summary>
        public uint uRelease;
        /// <summary>
        /// 建构编号
        /// </summary>
        public uint uBuild;
        /// <summary>
        /// 版本信息
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string sVersion;
        /// <summary>
        /// 版权信息
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string sCopyRight;
        /// <summary>
        /// 解码库能力集
        /// </summary>
        public uint uFunctionSet;
        /// <summary>
        /// 支持的输出图像格式
        /// </summary>
        public uint uPictureFormat;
        /// <summary>
        /// 输入码流格式
        /// </summary>
        public uint uStreamInType;
        /// <summary>
        /// 最大图像宽度(以像素为单位)
        /// </summary>
        public uint uPicWidth;
        /// <summary>
        /// 最大图像高度(以像素为单位)
        /// </summary>
        public uint uPicHeight;
        /// <summary>
        /// 最大参考帧数目
        /// </summary>
        public uint uBufNum;
        /// <summary>
        /// 保留字
        /// </summary>
        public uint uReserved;

    }

    /// <summary>
    /// 用户私有数据信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct hiH264_USERDATA_S
    {
        /// <summary>
        /// 用户数据类型
        /// </summary>
        public uint uUserDataType;
        /// <summary>
        /// 用户数据长度
        /// </summary>
        public uint uUserDataSize;
        /// <summary>
        /// 用户数据缓冲区
        /// </summary>
        public IntPtr pData;
        /// <summary>
        /// 指向下一段用户数据
        /// </summary>
        public IntPtr pNext;
    }


}
