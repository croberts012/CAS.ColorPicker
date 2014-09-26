using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

namespace CAS.ColorPicker
{
    [DefaultProperty("Color"),ValidationProperty("Color")]
    [ToolboxData("<{0}:ColorPicker runat=\"server\"></{0}:ColorPicker>")]
    [System.Drawing.ToolboxBitmap(typeof(ColorPicker),"Images.ColorPickerIcon.jpg")]
    public class ColorPicker : WebControl, IPostBackDataHandler
    {
        #region Events

        public event EventHandler ColorChanged;

        #endregion        

        //test

        #region Public Properties

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("#000000")]
        [Localizable(true)]
        public string Color
        {
            get
            {                
                return _Color;
            }

            set
            {
                _Color = value;
            }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue("false")]
        [Localizable(true)]
        public bool AutoPostBack 
        {
            get
            {
                return _AutoPostBack;
            }
            set
            {
                _AutoPostBack = value;
            }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue("0")]
        [Localizable(true)]
        public PopupPosition PopupPosition
        {
            get
            {
                return _PopupPosition;
            }

            set
            {
                _PopupPosition = value;
            }
        }

        #endregion

        #region Web.Control implementation        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
        }

        protected override void OnPreRender(EventArgs e)
        {            
            // Javascript
            string colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "CAS.ColorPicker.Javascript.ColorPicker.js");
            Page.ClientScript.RegisterClientScriptInclude("ColorPicker.js", colorFunctions);            

            // Create ColorPicker object
            string script = string.Format(@"
var colorPicker_{0} = new ColorPicker({{
FormWidgetAmountSliderHandleImage : '{1}',
TabRightActiveImage : '{2}',
TabRightInactiveImage : '{3}',
TabLeftActiveImage : '{4}',
TabLeftInactiveImage : '{5}',
AutoPostBack : {6},
AutoPostBackReference : ""{7}"",
PopupPosition : {8}
}});            
            ", ClientID
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "CAS.ColorPicker.Images.SliderHandle.gif")
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "CAS.ColorPicker.Images.TabRightActive.gif")
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "CAS.ColorPicker.Images.TabRightInactive.gif")
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "CAS.ColorPicker.Images.TabLeftActive.gif")
             , Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "CAS.ColorPicker.Images.TabLeftInactive.gif")             
             , AutoPostBack?"true":"false"
             , Page.ClientScript.GetPostBackEventReference(this,"")
             , (int)PopupPosition             
             );

            Page.ClientScript.RegisterStartupScript(Page.GetType(), String.Format("InitColorPicker_{0}", ClientID), script, true);
            if (!DesignMode && Page.Header != null)
            {
                RegisterCSSInclude(Page.Header);
            }            
        }

        protected override void LoadControlState(object savedState)
        {
            Color = (string)(savedState as object[])[0];
            AutoPostBack = (bool)(savedState as object[])[1];
            PopupPosition = (PopupPosition)(savedState as object[])[2];
        }

        protected override object SaveControlState()
        {
            object []saveState = new object[3];
            saveState[0] = Color;
            saveState[1] = AutoPostBack;
            saveState[2] = PopupPosition;
            return (object)saveState;
        }

        private void RegisterCSSInclude(Control target)
        {
            // CSS                   
            bool linkIncluded = false;
            foreach (Control c in target.Controls)
            {
                if (c.ID == "ControlPickerStyle")
                {
                    linkIncluded = true;
                }
            }
            if (!linkIncluded)
            {
                HtmlGenericControl csslink = new HtmlGenericControl("link");
                csslink.ID = "ControlPickerStyle";
                csslink.Attributes.Add("href", Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "CAS.ColorPicker.Styles.ColorPicker.css"));
                csslink.Attributes.Add("type", "text/css");
                csslink.Attributes.Add("rel", "stylesheet");
                csslink.EnableViewState = false;
                target.Controls.Add(csslink);
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            using (PlaceHolder plh = new PlaceHolder())
            {
                if (DesignMode || Page.Header == null)
                    RegisterCSSInclude(plh);
                Table table = new Table();
                table.CellPadding = 0;
                table.CellSpacing = 0;
                table.Rows.Add(new TableRow());
                table.Rows[0].Cells.Add(new TableCell());
                table.Rows[0].Cells.Add(new TableCell());
                table.Rows[0].Cells.Add(new TableCell());
                table.Rows[0].Cells[1].Style.Add(HtmlTextWriterStyle.PaddingRight, "5px");
                HtmlGenericControl txt = new HtmlGenericControl("input");
                txt.EnableViewState = false;
                txt.Attributes.Add("maxlength", "15");
                txt.Attributes.Add("size", "10");
                txt.Attributes.Add("value", Color);
                txt.Attributes.Add("id", ClientID);
                txt.Attributes.Add("name", UniqueID);
                txt.Attributes.CssStyle.Value = "height:17px;padding:2px;";
                table.Rows[0].Cells[0].Controls.Add(txt);
                HtmlGenericControl colorBar = new HtmlGenericControl("div");
                colorBar.EnableViewState = false;
                colorBar.Attributes.CssStyle.Add(HtmlTextWriterStyle.Height, "21px");
                colorBar.Attributes.CssStyle.Add(HtmlTextWriterStyle.Width, "5px");
                colorBar.Attributes.CssStyle.Add("border", "solid 1px #7f9db9");
                colorBar.Attributes.CssStyle.Add(HtmlTextWriterStyle.BackgroundColor, Color);
                table.Rows[0].Cells[1].Controls.Add(colorBar);
                HtmlInputImage btn = new HtmlInputImage();
                btn.Src = Page.ClientScript.GetWebResourceUrl(typeof(ColorPicker), "CAS.ColorPicker.Images.ColorPickerIcon.jpg");
                btn.Attributes.Add("onclick", string.Format("colorPicker_{0}.ShowColorPicker(this,document.getElementById('{1}'));return false;", ClientID, ClientID));
                btn.Attributes.CssStyle.Add(HtmlTextWriterStyle.ZIndex, "1");
                HtmlGenericControl container = new HtmlGenericControl("div");
                container.EnableViewState = false;
                container.Controls.Add(btn);
                container.Attributes.CssStyle.Add(HtmlTextWriterStyle.Position, "static");
                container.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, "block");
                table.Rows[0].Cells[2].Controls.Add(container);
                plh.Controls.Add(table);
                plh.RenderControl(output);
            }                      
        }               

        #endregion

        #region IPostBackDataHandler

        public bool LoadPostData(string postDataKey,NameValueCollection postCollection)
        {
            String presentValue = Color;
            String postedValue = postCollection[postDataKey];

            if (presentValue == null || !presentValue.Equals(postedValue))
            {
                Color = postedValue;
                return true;
            }
            return false;
        }

        public virtual void RaisePostDataChangedEvent()
        {
            OnColorChanged(EventArgs.Empty);
        }

        public void OnColorChanged(EventArgs e)
        {
            if (ColorChanged != null)
                ColorChanged(this, e);
        }

        #endregion

        #region Public static methods

        public static System.Drawing.Color StringToColor(string colorString)
        {
            System.Drawing.Color color;
            if (colorString[0] == '#' && colorString.Length < 8)
            {
                string s = colorString.Substring(1);
                while (s.Length != 6)
                {
                    s = string.Concat("0", s);
                }
                int red = Convert.ToInt32(s.Substring(0, 2), 16);
                int green = Convert.ToInt32(s.Substring(2, 2), 16);
                int blue = Convert.ToInt32(s.Substring(4, 2), 16);
                color = System.Drawing.Color.FromArgb(red, green, blue);
            }
            else
            {
                color = System.Drawing.Color.FromName(colorString);
            }
            return color;
        }
        public static string ColorToString(System.Drawing.Color color)
        {
            string result;
            if (color.IsKnownColor || color.IsNamedColor || color.IsSystemColor)
            {
                result = color.Name;
            }
            else
            {
                result = string.Concat("#", color.ToArgb().ToString("X").Substring(2));
            }
            return result;
        }

        #endregion        

        #region Private variables

        string _Color = "#000000";
        PopupPosition _PopupPosition = PopupPosition.BottomRight;
        bool _AutoPostBack;

        #endregion
    }
}
