using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using Android.Content;
using Android.Util;
using Android.Graphics;
using System.Net;
using Android.Widget;
using Android.App;

namespace VitaView {
    class GLView1 : AndroidGameView {
        static string ip = "0";
        System.Net.Sockets.UdpClient clientSocket; 
        IPEndPoint ep;
        public GLView1(Context context) : base(context) {
        }
        int [] textureIds = new int [1];
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Run();
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/SonryVitaView");
            Java.IO.File file = new Java.IO.File(dir, "ip.scf");
            Java.IO.FileReader fread = new Java.IO.FileReader(file);
            Java.IO.BufferedReader br = new Java.IO.BufferedReader(fread);
            ip = br.ReadLine();
            ep = new IPEndPoint(IPAddress.Parse(ip), 5000);
            GL.Enable(All.Texture2D);
            GL.GenTextures(1, textureIds);
            try {
                clientSocket = new System.Net.Sockets.UdpClient();
                clientSocket.Connect(ep);
            }
            catch(Exception ex) {
                Log.Verbose("Exception: ", "{0}", ex);
                Toast.MakeText(this.Context, "No Connection", ToastLength.Long).Show();
                Close();
            }
            
        }

        protected override void CreateFrameBuffer() {
            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try {
                Log.Verbose("GLCube", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex) {
                Log.Verbose("GLCube", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try {
                Log.Verbose("GLCube", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode(0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex) {
                Log.Verbose("GLCube", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        // This gets called on each frame render
        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            byte [] receivedData ;
            try {
                clientSocket.Send(new byte [] { 1, 2, 3, 4, 5 }, 5);
                receivedData = clientSocket.Receive(ref ep);
            }catch (Exception ex) {
                Log.Verbose("Exception: ", "{0}", ex);
                //Toast.MakeText(this.Context, "Close", ToastLength.Long).Show();
            }
            

            x++;
            if (x >= 1) {
                try {
                    receivedData = clientSocket.Receive(ref ep);
                    Bitmap bitmap = BitmapFactory.DecodeByteArray(receivedData, 0, receivedData.Length);
                    LoadTextureData(Context, bitmap, textureIds [0]);
                    bitmap.Recycle();
                }
                catch (Exception ex) {
                    Log.Verbose("Exception: ", "{0}", ex);
                }
            }

            GL.MatrixMode(All.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0f,1.0f, -1.5f, 1.5f, -1.0f, 1.0f);
            GL.MatrixMode(All.Modelview);
            //GL.Rotate(3.0f, 0.0f, 0.0f, 1.0f);

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Clear((uint)All.ColorBufferBit);

            GL.BindTexture(All.Texture2D, textureIds [0]);
            GL.EnableClientState(All.VertexArray);
            GL.EnableClientState(All.TextureCoordArray);

            GL.VertexPointer(2, All.Float, 0, square_vertices);
            GL.TexCoordPointer(2, All.Float, 0, text_vertices);
            GL.EnableClientState(All.VertexArray);
            //GL.ColorPointer(4, All.UnsignedByte, 0, square_colors);
            //GL.EnableClientState(All.ColorArray);

            GL.DrawArrays(All.TriangleStrip, 0, 4);
            GL.Finish();
            GL.DisableClientState(All.VertexArray);
            GL.DisableClientState(All.TextureCoordArray);
            SwapBuffers();
        }

        float [] square_vertices = {
            -1.0f, -1.5f,
            1.0f, -1.5f,
            -1.0f, 1.5f,
            1.0f, 1.5f,
        };
        float [] text_vertices = {
              -0.0f, 1.0f,
               1.0f, 1.0f,
              -0.0f, -0.0f,
               1.0f, -0.0f,
        };

        byte [] square_colors = {
            255, 255,   0, 255,
            0,   255, 255, 255,
            0,     0,    0,  0,
            255,   0,  255, 255,
        };


        int x = 0;
        private async void dataReceive() {
            clientSocket.Send(new byte [] { 1, 2, 3, 4, 5 }, 5);
            //Toast.MakeText(this.Context, "Start", ToastLength.Long).Show();
            var receivedData = clientSocket.Receive(ref ep);
            //Toast.MakeText(this.Context, "Pass", ToastLength.Long).Show();
            x++;
            while (true) {
                await System.Threading.Tasks.Task.Delay(1);

                if (x >= 1) {
                    try {
                        receivedData = clientSocket.Receive(ref ep);
                        Bitmap bitmap = BitmapFactory.DecodeByteArray(receivedData, 0, receivedData.Length);
                        LoadTextureData(Context, bitmap, textureIds [0]);
                        bitmap.Recycle();
                    }
                    catch (Exception ex) {
                        Log.Verbose("asdjasd", "{0}", ex);
                    }
                }
            }

        }

        void LoadTextureData(Context context, Bitmap b, int tex_id) {
            GL.BindTexture(All.Texture2D, tex_id);

            // setup texture parameters
            GL.TexParameterx(All.Texture2D, All.TextureMagFilter, (int)All.Linear);
            GL.TexParameterx(All.Texture2D, All.TextureMinFilter, (int)All.Linear);
            GL.TexParameterx(All.Texture2D, All.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameterx(All.Texture2D, All.TextureWrapT, (int)All.ClampToEdge);

            Android.Opengl.GLUtils.TexImage2D((int)All.Texture2D, 0, b, 0);
        }

        
    }
}
