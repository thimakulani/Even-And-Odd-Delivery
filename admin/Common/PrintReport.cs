using Android.Content;
using Android.Content.Res;
using Android.Widget;
using Java.IO;

namespace admin.Common
{
    class PrintReport
    {
        public static string GetAppPath(Context context)
        {
            File dir = new File(Xamarin.Essentials.FileSystem.AppDataDirectory
                + File.Separator
                + context.GetString(Resource.String.app_name)
                + File.Separator);
            if (!dir.Exists())
            {
                dir.Mkdir();
            }

            return dir.Path + File.Separator;
        }
        public static void WriteFileToStorage(Context context, string fileName)
        {
            AssetManager asset = context.Assets;
            if (new File(GetFilePath(context, fileName)).Exists())
            {
                return;
            }
            try
            {
                var input = asset.Open(fileName);
                var output = new FileOutputStream(GetFilePath(context, fileName));
                byte[] buffer = new byte[8 * 1024];
                int length;
                while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, length);
                }
            }
            catch (FileNotFoundException ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
            }
            catch (IOException ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
            }
        }

        public static string GetFilePath(Context context, string fileName)
        {

            return context.FilesDir + File.Separator + fileName;
        }
    }
}