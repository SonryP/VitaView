using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

namespace VitaView {
    [Activity(Label = "VitaView",
        MainLauncher = true,
        Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden
#if __ANDROID_11__
		,HardwareAccelerated=false
#endif
      ,ScreenOrientation = ScreenOrientation.Landscape  ,Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity {
        GLView1 view;
        TextView label1;
        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            //view = new GLView1(this);
            //SetContentView(view);
            SetContentView(Resource.Layout.Start);
            Button btn1 = FindViewById<Button>(Resource.Id.button1);
            Button btn2 = FindViewById<Button>(Resource.Id.button2);
            EditText text1 = FindViewById<EditText>(Resource.Id.editText1);
            label1 = FindViewById<TextView>(Resource.Id.textView5);
            label1.Text = reFile();
            btn1.Click += delegate {
                saveFl(text1.Text);
                label1.Text = reFile();
            };
            btn2.Click += delegate {
                view = new GLView1(this);
                SetContentView(view);
            };
        }

        /*protected override void OnPause() {
            base.OnPause();
            view.Pause();
        }

        protected override void OnResume() {
            base.OnResume();
            view.Resume();
        }*/
        private void saveFl(string texto) {
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/SonryVitaView");
            dir.Mkdirs();
            Java.IO.File file = new Java.IO.File(dir, "ip.scf");
            if (!file.Exists()) {
                file.CreateNewFile();
                file.Mkdir();
                Java.IO.FileWriter writer = new Java.IO.FileWriter(file);
                writer.Write(texto);
                writer.Flush();
                writer.Close();
                Toast.MakeText(this, "Successfully Saved", ToastLength.Long).Show();
            } else {
                Java.IO.FileWriter writer = new Java.IO.FileWriter(file);
                writer.Write(texto);
                writer.Flush();
                writer.Close();
                Toast.MakeText(this, "Successfully Edited", ToastLength.Long).Show();
            }
        }
        private string reFile() {
            string ip = "0";
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/SonryVitaView");
            Java.IO.File file = new Java.IO.File(dir, "ip.scf");
            if (!file.Exists()) {
                Toast.MakeText(this, "Remember to store an IP", ToastLength.Long).Show();
                return "No IP Saved";
            } else {
                Java.IO.FileReader fread = new Java.IO.FileReader(file);
                Java.IO.BufferedReader br = new Java.IO.BufferedReader(fread);
                ip = br.ReadLine();
                fread.Close();
                return ip;
            }

        }

    }
}

