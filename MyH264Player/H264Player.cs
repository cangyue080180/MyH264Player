using Renderer.Core;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MyH264Player
{
    public class H264Player
    {
        #region 解码器相关变量声明

        public const int HI_SUCCESS = 0;

        public const int HI_FAILURE = -1;

        public const int HI_LITTLE_ENDIAN = 1234;

        public const int HI_BIG_ENDIAN = 4321;

        public const int HI_DECODER_SLEEP_TIME = 60000;

        public const int HI_H264DEC_OK = 0;

        public const int HI_H264DEC_NEED_MORE_BITS = -1;

        public const int HI_H264DEC_NO_PICTURE = -2;

        public const int HI_H264DEC_ERR_HANDLE = -3;

        static double[,] YUV2RGB_CONVERT_MATRIX = new double[3, 3] { { 1, 0, 1.4022 }, { 1, -0.3456, -0.7145 }, { 1, 1.771, 0 } };

        #endregion
        private ConcurrentQueue<byte> streamBuf = null;
        //码流结束,0==false,1=true
        private int isStop = 0;
        private int width, height;
        private D3DImageSource d3dSource;
        /// <summary>
        /// 视频播放器构造函数
        /// </summary>
        /// <param name="stream">要播放的流</param>
        /// <param name="width">视频像素宽</param>
        /// <param name="height">视频像素高</param>
        /// <param name="d3dSource">视频显示源</param>
        public H264Player(ConcurrentQueue<byte> stream,int width,int height, D3DImageSource d3dSource)
        {
            this.streamBuf = stream;
            this.width = width;
            this.height = height;
            this.d3dSource = d3dSource;
        }
        //开始播放
        public void Start()
        {
            isStop = 0;
            Task.Run(() =>
            {
                DecoMethod();
            });
        }
       // 停止播放
        public void Stop()
        {
            isStop = 1;
        }
        
        //解码刷新显示
        private void DecoMethod()
        {
            //初始化
            // 这是解码器输出图像信息
            hiH264_DEC_FRAME_S _decodeFrame = new hiH264_DEC_FRAME_S();
            // 这是解码器属性信息
            hiH264_DEC_ATTR_S decAttr = new hiH264_DEC_ATTR_S();
            decAttr.uPictureFormat = 0;
            decAttr.uStreamInType = 0;
            /* 解码器最大图像宽高, D1图像(1280x720) */
            decAttr.uPicWidthInMB = (uint)width >>4;
            decAttr.uPicHeightInMB = (uint)height >>4;
            /* 解码器最大参考帧数: 16 */
            decAttr.uBufNum = 8;
            /* bit0 = 1: 标准输出模式; bit0 = 0: 快速输出模式 */
            /* bit4 = 1: 启动内部Deinterlace; bit4 = 0: 不启动内部Deinterlace */
            decAttr.uWorkMode = 0x10;
            //创建、初始化解码器句柄
            IntPtr _decHandle = H264Dec.Hi264DecCreate(ref decAttr);
            //解码结束
            bool isEnd = false;
            int bufferLen = 0x8000;
            //码流段
            byte[] buf = new byte[bufferLen];
            while (!isEnd)
            {
                //获取一段码流,积累一定缓存量再解
                if (streamBuf.Count >= bufferLen || isStop == 1)
                {
                    byte tempByte;
                    int j = 0;
                    for (int i = 0; i < bufferLen; i++)
                    {
                        if (streamBuf.TryDequeue(out tempByte))
                            buf[j++] = tempByte;
                        else
                        {
                            break;
                        }
                    }
                    IntPtr pData = Marshal.AllocHGlobal(j);
                    Marshal.Copy(buf, 0, pData, j);
                    int result = 0;
                    result = H264Dec.Hi264DecFrame(_decHandle, pData, (UInt32)j, 0, ref _decodeFrame, (uint)isStop);
                    while (HI_H264DEC_NEED_MORE_BITS != result)
                    {
                        if (HI_H264DEC_NO_PICTURE == result)
                        {
                            isEnd = true;
                            break;
                        }
                        if (HI_H264DEC_OK == result)/* 输出一帧图像 */
                        {
                            //获取yuv
                            //UInt32 tempWid = _decodeFrame.uWidth;
                            //UInt32 tempHeig = _decodeFrame.uHeight;
                            //UInt32 yStride = _decodeFrame.uYStride;
                            //UInt32 uvStride = _decodeFrame.uUVStride;
                            //byte[] y = new byte[tempHeig * yStride];
                            //byte[] u = new byte[tempHeig * uvStride / 2];
                            //byte[] v = new byte[tempHeig * uvStride / 2];
                            //Marshal.Copy(_decodeFrame.pY, y, 0, y.Length);
                            //Marshal.Copy(_decodeFrame.pU, u, 0, u.Length);
                            //Marshal.Copy(_decodeFrame.pV, v, 0, v.Length);

                            //转为yv12格式
                            //byte[] yuvBytes = new byte[y.Length + u.Length + v.Length];
                            //Array.Copy(y, 0, yuvBytes, 0, y.Length);
                            //Array.Copy(v, 0, yuvBytes, y.Length , v.Length);
                            //Array.Copy(u, 0, yuvBytes, y.Length + v.Length, u.Length);
                            //更新显示
                            this.d3dSource.Render(_decodeFrame.pY, _decodeFrame.pU, _decodeFrame.pV);
                        }
                        /* 继续解码剩余H.264码流 */
                        result = H264Dec.Hi264DecFrame(_decHandle, IntPtr.Zero, 0, 0, ref _decodeFrame, (uint)isStop);
                    }
                }
             //   System.Threading.Thread.Sleep(5);
            }
            /* 销毁解码器 */
            H264Dec.Hi264DecDestroy(_decHandle);
        }

    }
}
