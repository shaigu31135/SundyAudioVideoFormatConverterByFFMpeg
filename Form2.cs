using System;
using System.Text;
using System.Windows.Forms;

namespace FFMPEG快速格式转换 {
    public partial class Form2 : Form {

        private string[] cmd;

        private string ffmpegPath = Application.StartupPath + @"\FFMPEG\bin\ffmpeg.exe";

        private string[] fileFormatString = { "mp4", "flv", "avi", "mkv", "mov", "wmv", "mp3", "flac", "wav", "wma", "aac", "ogg" };
        private enum fileFormat { ff_mp4, ff_flv, ff_avi, ff_mkv, ff_mov, ff_wmv, ff_mp3, ff_flac, ff_wav, ff_wma, ff_aac, ff_ogg, ff_null }
        private string[] presets = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo" };
        private string[] tunes = { "", "film", "animation", "grain", "stillimage", "psnr", "ssim", "fastdecode", "zerolatency" };

        private fileFormat formatString2fileFormat ( string f ) {
            switch ( f ) {
                case "mp4":
                    return fileFormat.ff_mp4;
                case "flv":
                    return fileFormat.ff_flv;
                case "avi":
                    return fileFormat.ff_avi;
                case "mkv":
                    return fileFormat.ff_mkv;
                case "mov":
                    return fileFormat.ff_mov;
                case "wmv":
                    return fileFormat.ff_wmv;
                case "mp3":
                    return fileFormat.ff_mp3;
                case "flac":
                    return fileFormat.ff_flac;
                case "wav":
                    return fileFormat.ff_wav;
                case "wma":
                    return fileFormat.ff_wma;
                case "aac":
                    return fileFormat.ff_aac;
                case "ogg":
                    return fileFormat.ff_ogg;
                default:
                    return fileFormat.ff_null;
            }
        }


        private bool isAudioFile ( string file ) {
            StringBuilder format = new StringBuilder ();
            format.Append ( ".mp3" ).Append ( ".flac" ).Append ( ".ape" ).Append ( ".m4a" ).Append ( ".wav" ).Append ( ".wma" ).Append ( ".aac" ).Append ( ".ogg" );
            return format.ToString ().Contains ( System.IO.Path.GetExtension ( file ) );
        }

        private void ffRpegProcess ( ref string[] paths, fileFormat ff, int outputPathPos, bool exitApplicationAfterConvertCompleted = true ) {
            StringBuilder outputPath = new StringBuilder ();
            StringBuilder outputName = new StringBuilder ();
            StringBuilder audioEncoder = new StringBuilder ();
            StringBuilder cmd = new StringBuilder ();
            foreach ( string s in paths ) {
                if ( isCorrectFormat ( s ) ) {
                    outputPath.Clear ();
                    outputName.Clear ();
                    audioEncoder.Clear ();
                    cmd.Clear ();
                    if ( outputPathPos == 0 ) {
                        outputPath.Append ( System.IO.Path.GetDirectoryName ( s ) );
                    } else {
                        if ( !System.IO.Directory.Exists ( Application.StartupPath + @"\Output" ) ) System.IO.Directory.CreateDirectory ( Application.StartupPath + @"\Output" );
                        outputPath.Append ( Application.StartupPath + @"\Output" );
                    }
                    outputName.Append ( System.IO.Path.GetFileNameWithoutExtension ( s ) ).Append ( getFileFormat ( ff ) );

                    switch ( ff ) {
                        case fileFormat.ff_mp4:
                            if ( !isAudioFile ( s ) ) cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:v libx264 -preset slow -tune fastdecode -cq 16 -rc vbr_hq -profile main -c:a aac -b:a 320k -movflags +faststart -x265-params level=6.2:high-tier=1 -map_metadata 0 \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_flv:
                            if ( !isAudioFile ( s ) ) cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:v libx264 -preset medium -rc vbr_hq -cq 21 -b:v 4000k -maxrate 8000k -bufsize 8000k -c:a aac -b:a 160k -profile:v high -movflags +faststart -g 120 -bf 3 -map_metadata 0 \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_avi:
                            if ( !isAudioFile ( s ) ) cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:v libx264 -preset veryfast -rc vbr_hq -cq 28 -b:v 1500k -maxrate 2500k -bufsize 3000k -r 60 -c:a aac -b:a 92k -g 240 -x264-params no-scenecut=1 -map_metadata 0 \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_mkv:
                            if ( !isAudioFile ( s ) ) cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:v libx264 -preset veryfast -rc vbr_hq -cq 16 -b:v 1500k -maxrate 2500k -bufsize 3000k -r 60 -c:a aac -b:a 92k -g 240 -x264-params no-scenecut=1 -map_metadata 0 \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_mov:
                            if ( !isAudioFile ( s ) ) cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:v libx264 -preset veryslow -qp 0 -rc constqp -profile:v high444 -pix_fmt yuv444p16le -bf 0 -refs 6 -coder cabac -c:a pcm_s24le -movflags +faststart -f mov -map_metadata 0 \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_wmv:
                            if ( !isAudioFile ( s ) ) cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:v libx264 -preset slow -cq 0 -bf 3 -rc vbr_hq -c:a aac -b:a 320k -map_metadata 0 \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_mp3:
                            cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:a libmp3lame -q:a 0 -b:a 320k -ar 48000 -precision 24 -map_metadata 0 -vn \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_flac:
                            cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:a flac -compression_level 0 -sample_fmt s32 -ar 192000 -bits_per_raw_sample 24 -map_metadata 0 -vn \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_wav:
                            cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:a pcm_s32le -ar 192000 -ac 2 -map_metadata 0 -vn \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_wma:
                            cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:a wmav2 -b:a 320k -ac 2 -surround_mix_level 1.5 -compression_level 100 -strict experimental -map_metadata 0 -vn \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_aac:
                            cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:a aac -b:a 512k -map_metadata 0 -vn \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        case fileFormat.ff_ogg:
                            cmd.Append ( $"-y -hwaccel auto -threads 0 -i \"{s}\" -c:a libvorbis -qscale:a 6 -compression_level 10 -frame_duration 60 -map_metadata 0 -vn \"{outputPath.ToString () + @"\" + outputName.ToString ()}\"" );
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine ( cmd.ToString () );
                    if ( System.IO.Path.GetExtension ( s ) != getFileFormat ( ff ) ) ffConvert ( cmd.ToString () );
                }
            }
            if ( exitApplicationAfterConvertCompleted ) Application.Exit ();
        }

        private void ffConvert ( string cmdLine ) {
            System.Diagnostics.Process process = new System.Diagnostics.Process ();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo ();
            try {
                if ( !checkBox2.Checked ) startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = ffmpegPath;
                startInfo.Arguments = cmdLine;
                process.StartInfo = startInfo;
                process.Start ();
            } catch ( Exception ex ) {
                MessageBox.Show ( ex.Message );
            }
        }

        private string getFileFormat ( fileFormat ff ) {
            switch ( ff ) {
                case fileFormat.ff_mp4: return ".mp4";
                case fileFormat.ff_flv: return ".flv";
                case fileFormat.ff_avi: return ".avi";
                case fileFormat.ff_mkv: return ".mkv";
                case fileFormat.ff_mov: return ".mov";
                case fileFormat.ff_wmv: return ".wmv";
                case fileFormat.ff_mp3: return ".mp3";
                case fileFormat.ff_flac: return ".flac";
                case fileFormat.ff_wav: return ".wav";
                case fileFormat.ff_wma: return ".wma";
                case fileFormat.ff_aac: return ".aac";
                case fileFormat.ff_ogg: return ".ogg";
                default: return "";
            }
        }

        private bool isCorrectFormat ( string s ) {
            StringBuilder format = new StringBuilder ();
            format.Append ( ".mp4" ).Append ( ".flv" ).Append ( ".avi" ).Append ( ".mkv" ).Append ( ".mov" ).Append ( ".wmv" ).Append ( ".mp3" ).Append ( ".flac" ).Append ( ".ape" ).Append ( ".m4a" ).Append ( ".wav" ).Append ( ".wma" ).Append ( ".aac" ).Append ( ".ogg" );
            return format.ToString ().Contains ( System.IO.Path.GetExtension ( s ) );
        }

        private string getButtonText ( object sender ) {
            Button button = sender as Button;
            if ( button != null ) return button.Text;
            else return "";
        }

        public Form2 ( string[] args ) {
            InitializeComponent ();
            cmd = args;
        }

        private void button1_Click ( object sender, System.EventArgs e ) {
            Form1 form1 = new Form1 ();
            this.Hide ();
            form1.ShowDialog ( this );
            this.Close ();
        }

        private void button2_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button3_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button4_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button5_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button6_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button7_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button13_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button12_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button11_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button10_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button9_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void button8_Click ( object sender, EventArgs e ) {
            if ( cmd.Length > 0 ) ffRpegProcess ( ref cmd, formatString2fileFormat ( getButtonText ( sender ).ToLower ().Replace ( "转换为", "" ) ), 0 );
        }

        private void Form2_Load ( object sender, EventArgs e ) {

        }
    }
}
