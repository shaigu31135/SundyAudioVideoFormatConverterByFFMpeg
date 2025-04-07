using Microsoft.Win32;
using System;
using System.Text;
using System.Windows.Forms;

namespace FFMPEG快速格式转换 {
    public partial class Form1 : Form {
        public Form1 () {
            InitializeComponent ();
        }

        private string ffmpegPath = Application.StartupPath + @"\FFMPEG\bin\ffmpeg.exe";

        private string[] fileFormatString = { "mp4", "flv", "avi", "mkv", "mov", "wmv", "mp3", "flac", "wav", "wma", "aac", "ogg" };
        private enum fileFormat { ff_mp4, ff_flv, ff_avi, ff_mkv, ff_mov, ff_wmv, ff_mp3, ff_flac, ff_wav, ff_wma, ff_aac, ff_ogg }
        private string[] presets = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo" };
        private string[] tunes = { "", "film", "animation", "grain", "stillimage", "psnr", "ssim", "fastdecode", "zerolatency" };

        private void ffRpegProcess ( ref string[] paths, fileFormat ff, int outputPathPos ) {
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
                    if ( checkBox3.Checked && Convert.ToInt32 ( ff ) <= 5 ) {//视频转换，启用高级模式
                        cmd.Append ( checkBox6.Checked ? "-y " : "" ).Append ( checkBox4.Checked ? "-hwaccel auto " : "" ).Append ( checkBox5.Checked ? "-threads 0 " : "" ).Append ( $"-i \"{s}\" " ).Append ( $"-c:v {comboBox2.Text} " ).Append ( $"-preset {presets[trackBar1.Value]} " ).Append ( $"-cq {label21.Text} " ).Append ( comboBox3.SelectedIndex != 0 ? $"-tune {tunes[comboBox3.SelectedIndex]} " : "" ).Append ( $"-c:a {comboBox5.Text} " ).Append ( "-map_metadata 0 " ).Append ( $"\"{outputPath.ToString ()}\\{outputName.ToString ()}\"" );
                    } else if ( checkBox7.Checked && Convert.ToInt32 ( ff ) >= 6 ) {
                        switch ( ff ) {
                            case fileFormat.ff_mp3:
                                audioEncoder.Append ( "libmp3lame" );
                                if ( comboBox4.SelectedIndex < 5 ) comboBox4.SelectedIndex = 5;
                                break;
                            case fileFormat.ff_flac:
                                audioEncoder.Append ( "flac" );
                                break;
                            case fileFormat.ff_wav:
                                audioEncoder.Append ( "pcm_s32le" );
                                break;
                            case fileFormat.ff_wma:
                                audioEncoder.Append ( "wmav2" );
                                break;
                            case fileFormat.ff_aac:
                                audioEncoder.Append ( "aac" );
                                if ( comboBox4.SelectedIndex < 2 ) comboBox4.SelectedIndex = 2;
                                break;
                            case fileFormat.ff_ogg:
                                audioEncoder.Append ( "libvorbis" );
                                break;
                        }
                        cmd.Append ( checkBox8.Checked ? "-y " : "" ).Append ( checkBox10.Checked ? "-hwaccel auto " : "" ).Append ( checkBox9.Checked ? "-threads 0 " : "" ).Append ( $"-i \"{s}\" " ).Append ( $"-c:a {audioEncoder.ToString ()} " ).Append ( $"-ar {comboBox4.Text} " ).Append ( $"-b:a {comboBox6.Text} " ).Append ( $"-q:a 0 " ).Append ( comboBox7.SelectedIndex == 0 ? "" : $"-ac {comboBox7.SelectedIndex.ToString ()} " ).Append ( "-map_metadata 0 -vn " ).Append ( $"\"{outputPath.ToString ()}\\{outputName.ToString ()}\"" );
                    } else {
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
                    }
                    Console.WriteLine ( cmd.ToString () );
                    if ( System.IO.Path.GetExtension ( s ) != getFileFormat ( ff ) ) ffConvert ( cmd.ToString () );
                }
            }
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

        private bool isAudioFile ( string file ) {
            StringBuilder format = new StringBuilder ();
            format.Append ( ".mp3" ).Append ( ".flac" ).Append ( ".ape" ).Append ( ".m4a" ).Append ( ".wav" ).Append ( ".wma" ).Append ( ".aac" ).Append ( ".ogg" );
            return format.ToString ().Contains ( System.IO.Path.GetExtension ( file ) );
        }

        private void checkBox1_CheckedChanged ( object sender, EventArgs e ) {
            this.TopMost = checkBox1.Checked;
        }

        private void label1_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label1_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_mp4, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label3_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_flv, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label3_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label5_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_avi, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label5_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label4_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_mkv, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label4_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label7_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_mov, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label7_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label6_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label6_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_wmv, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void Form1_Load ( object sender, EventArgs e ) {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged ( object sender, EventArgs e ) {
            panel2.Enabled = comboBox1.SelectedIndex == comboBox1.Items.Count - 1;
            label24.Visible = comboBox1.Text.Contains ( "快" );
            switch ( comboBox1.SelectedIndex ) {
                case 0://均衡
                    checkBox4.Checked = true;
                    checkBox5.Checked = true;
                    checkBox6.Checked = true;
                    comboBox2.SelectedIndex = 0;
                    trackBar1.Value = 5;
                    trackBar2.Value = 26;
                    label21.Text = 26.ToString ();
                    comboBox3.SelectedIndex = 0;
                    comboBox5.SelectedIndex = 3;
                    break;
                case 1://最好
                    checkBox4.Checked = true;
                    checkBox5.Checked = true;
                    checkBox6.Checked = true;
                    comboBox2.SelectedIndex = 4;
                    trackBar1.Value = 9;
                    trackBar2.Value = 0;
                    label21.Text = 0.ToString ();
                    comboBox3.SelectedIndex = 1;
                    comboBox5.SelectedIndex = 2;
                    break;
                case 2://更好
                    checkBox4.Checked = true;
                    checkBox5.Checked = true;
                    checkBox6.Checked = true;
                    comboBox2.SelectedIndex = 2;
                    trackBar1.Value = 7;
                    trackBar2.Value = 14;
                    label21.Text = 14.ToString ();
                    comboBox3.SelectedIndex = 2;
                    comboBox5.SelectedIndex = 2;
                    break;
                case 3://更快
                    checkBox4.Checked = true;
                    checkBox5.Checked = true;
                    checkBox6.Checked = true;
                    comboBox2.SelectedIndex = 0;
                    trackBar1.Value = 3;
                    trackBar2.Value = 38;
                    label21.Text = 38.ToString ();
                    comboBox3.SelectedIndex = 7;
                    comboBox5.SelectedIndex = 4;
                    break;
                case 4://最快
                    checkBox4.Checked = true;
                    checkBox5.Checked = true;
                    checkBox6.Checked = true;
                    comboBox2.SelectedIndex = 0;
                    trackBar1.Value = 0;
                    trackBar2.Value = 51;
                    label21.Text = 51.ToString ();
                    comboBox3.SelectedIndex = 7;
                    comboBox5.SelectedIndex = 5;
                    break;
            }
        }

        private void checkBox3_CheckedChanged ( object sender, EventArgs e ) {
            groupBox17.Visible = checkBox3.Checked;
            label17.Visible = checkBox3.Checked;
            comboBox1.Visible = checkBox3.Checked;
        }

        private void trackBar2_Scroll ( object sender, EventArgs e ) {
            label21.Text = trackBar2.Value.ToString ();
        }

        private void label13_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label13_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_mp3, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label12_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_flac, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label12_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label10_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label10_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_wav, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label8_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_wma, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label8_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label15_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_aac, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label15_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void label14_DragDrop ( object sender, DragEventArgs e ) {
            string[] paths = (string[]) (System.Array) e.Data.GetData ( DataFormats.FileDrop );
            if ( paths.Length == 0 ) return;
            ffRpegProcess ( ref paths, fileFormat.ff_ogg, Convert.ToInt32 ( radioButton1.Checked ? "0" : "1" ) );
        }

        private void label14_DragEnter ( object sender, DragEventArgs e ) {
            if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }

        private void checkBox7_CheckedChanged ( object sender, EventArgs e ) {
            groupBox10.Visible = checkBox7.Checked;
        }

        private void button1_Click ( object sender, EventArgs e ) {
            for ( int i = 0; i < fileFormatString.Length; ++i ) {
                if ( RegistryOperation.isRegExist ( Registry.ClassesRoot, @"SystemFileAssociations\." + fileFormatString[i] + @"\shell\SundyAudioVideoFormatConverterByFFMpeg" ) ) {
                    //已存在，删除
                    RegistryOperation.DelReg ( Registry.ClassesRoot, @"SystemFileAssociations\." + fileFormatString[i] + @"\shell\SundyAudioVideoFormatConverterByFFMpeg" );
                } else {
                    //不存在，添加
                    RegistryOperation.AddReg ( Registry.ClassesRoot, @"SystemFileAssociations\." + fileFormatString[i] + @"\shell\SundyAudioVideoFormatConverterByFFMpeg", "音视频格式转换" );
                }
            }

            if ( !RegistryOperation.isRegExist ( Registry.ClassesRoot, @"SystemFileAssociations\.ogg\shell\SundyAudioVideoFormatConverterByFFMpeg" ) ) {
                //已删除
                MessageBox.Show ( "已经从右键菜单中移除。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information );
            } else {
                //已添加
                MessageBox.Show ( "已经集成到右键菜单。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
        }


    }
}
