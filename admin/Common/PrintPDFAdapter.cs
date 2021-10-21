using Android.Content;
using Android.OS;
using Android.Print;
using Android.Widget;
using Java.IO;
using System;

namespace admin.Common
{
    public class PrintReportPDFAdapter : PrintDocumentAdapter
    {
        private readonly Context context;
        private readonly string path;
        private readonly string filename;
        public PrintReportPDFAdapter(Context context, string path, string filename)
        {
            this.context = context;
            this.path = path;
            this.filename = filename;
        }

        public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
        {
            if (cancellationSignal.IsCanceled)
            {
                callback.OnLayoutCancelled();
            }
            else
            {
                PrintDocumentInfo.Builder builder = new PrintDocumentInfo.Builder(filename);
                builder.SetPageCount(PrintDocumentInfo.PageCountUnknown)
                    .Build();
                callback.OnLayoutFinished(builder.Build(), !newAttributes.Equals(oldAttributes));
            }
        }

        public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
        {
            InputStream input = null;
            OutputStream output = null;
            try
            {
                File file = new File(path);
                input = new FileInputStream(file);
                output = new FileOutputStream(destination.FileDescriptor);

                byte[] buffer = new byte[8 * 1024];

                int length;

                while ((length = input.Read(buffer)) >= 0 && !cancellationSignal.IsCanceled)

                    output.Write(buffer, 0, length);
                if (cancellationSignal.IsCanceled)
                    callback.OnWriteCancelled();
                else
                    callback.OnWriteFinished(new PageRange[] { PageRange.AllPages });


            }
            catch (Exception ex)
            {
                Toast.MakeText(context, "adapter error" + ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                try
                {
                    input.Close();
                    output.Close();
                }
                catch (IOException ex)
                {
                    Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
                }
                catch (NullReferenceException ex)
                {
                    Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
                }
            }
        }
    }
}