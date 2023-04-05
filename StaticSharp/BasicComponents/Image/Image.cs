using ImageMagick;
using Scopes;
using StaticSharp.Gears;
using StaticSharp.Html;
using System.Runtime.CompilerServices;


namespace StaticSharp {


    namespace Js {
        public interface Image : AspectBlock {
            //public double Aspect  { get; }
        }
    }


    namespace Gears {
        public class ImageBindings<FinalJs> : AspectBlockBindings<FinalJs> {
            //public Binding<double> Aspect { set { Apply(value); } }
        }
    }



    [Mix(typeof(ImageBindings<Js.Image>))]
    [ConstructorJs]
    public partial class Image : AspectBlock, IMainVisual {

        public enum TEmbed { 
            Image,
            Thumbnail,
            None
        }

        protected override string TagName => "image-block";

        protected Genome<IAsset> assetGenome;

        public TEmbed Embed { get; set; } = TEmbed.Thumbnail;


        public Image(Image other, int callerLineNumber, string callerFilePath)
            : base(other, callerLineNumber, callerFilePath) {
            assetGenome = other.assetGenome;
        }
        public Image(Genome<IAsset> assetGenome, [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "") : base(callerLineNumber, callerFilePath) {
            this.assetGenome = assetGenome;
        }
        
        public Image(string pathOrUrl, [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "") : base(callerLineNumber, callerFilePath) {
            assetGenome = AssetGenome.GenomeFromPathOrUrl(pathOrUrl, callerFilePath);
        }

        /*public override void AddRequiredInclues(IIncludes includes) {
            base.AddRequiredInclues(includes);
            includes.Require(new Script(ThisFilePathWithNewExtension("js")));
        }*/
        IAsset GetSource() {
            string[] webExtensions = { ".jpg", ".jpeg", ".png", ".svg" };

            var source = assetGenome.Result;
            if (!webExtensions.Contains(source.Extension)) {
                source = new JpegGenome(assetGenome).Result;
            }
            return source;
        }


        MagickImageInfo GetImageInfo(IAsset source) {
            return new MagickImageInfo(source.Data);
        }


        public override void ModifyTagAndScript(Context context, Tag tag, Group script) {
            base.ModifyTagAndScript(context, tag, script);

            var contentId = context.CreateId();
            
            var source = GetSource();
            var imageInfo = GetImageInfo(source);

            SetNativeSize(script, tag.Id, imageInfo.Width, imageInfo.Height);

            script.Add($"{tag.Id}.content = {TagToJsValue(contentId)}");


            //tag["data-width"] = imageInfo.Width;
            //tag["data-height"] = imageInfo.Height;

            string url;



            void AddSimpleImage() {
                tag.Add(new Tag("content", contentId) {
                    new Tag("img") {
                        ["width"] = "100%",
                        ["height"] = "100%",
                        ["src"] = url,
                    }
                });
            }

            if (Embed == TEmbed.Image) {
                if (source.GetMediaType() == "image/svg+xml") {
                    url = source.GetDataUrlXml();
                } else {
                    url = source.GetDataUrlBase64();
                }
                AddSimpleImage();
                return;
            }

            url = context.PathFromHostToCurrentPage.To(context.AddAsset(source)).ToString();
            if (Embed == TEmbed.None) {
                AddSimpleImage();
                return;
            }


            var thumbnail = new ThumbnailGenome(assetGenome).Result;
            var thumbnailUrlBase64 = thumbnail.GetDataUrlBase64();

            var thumbnailSvgDefTag = Svg.InlineImage(thumbnailUrlBase64);
            var thumbnailId = context.SvgDefs.Add(thumbnailSvgDefTag);

            var hBlurId = context.SvgDefs.Add(Svg.BlurFilter(0.5f, 0));
            var vBlurId = context.SvgDefs.Add(Svg.BlurFilter(0, 0.5f));


            var quantizeSettings = new QuantizeSettings() {
                Colors = 4,
                ColorSpace = ColorSpace.RGB,
                DitherMethod = DitherMethod.No,
                MeasureErrors = false,
                //TreeDepth = 0
            };

            var thumbnailImageInfo = GetImageInfo(thumbnail);

            tag.Add(new Tag("content", contentId) {

                new Tag("svg"){
                    Id = "thumbnail",
                    ["width"] = "100%",
                    ["height"] = "100%",
                    ["viewBox"] = $"0 0 {thumbnailImageInfo.Width} {thumbnailImageInfo.Height}",
                    ["preserveAspectRatio"] = "none",
                    Style = {
                        ["overflow"] = "hidden",
                        ["display"] = "none", //for not(.js)
                    },
                    Children = {
                        new Tag("use"){
                            ["href"]="#"+thumbnailId,
                        },
                        new Tag("use"){
                            ["href"]="#"+thumbnailId,
                            ["filter"] = $"url(#{vBlurId})"
                        },
                        new Tag("g"){
                            ["filter"] = $"url(#{hBlurId})",
                            Children = {
                                new Tag("use"){
                                    ["href"]="#"+thumbnailId,
                                },
                                new Tag("use"){
                                    ["href"]="#"+thumbnailId,
                                    ["filter"] = $"url(#{vBlurId})"
                                }
                            }
                        }
                    }
                },
                new Tag("img") {
                    ["width"] = "100%",
                    ["height"] = "100%",
                    ["src"] = url
                }
            }
            );

        }


        void IMainVisual.GetMeta(Dictionary<string, string> meta, Context context) {
            var source = GetSource();
            var imageInfo = GetImageInfo(source);
            var url = (context.BaseUrl + context.AddAsset(source)).ToString();
            meta["og:image"] = url;
            meta["og:image:width"] = imageInfo.Width.ToString();
            meta["og:image:height"] = imageInfo.Height.ToString();

            meta["twitter:image"] = url;
        }
    }

}