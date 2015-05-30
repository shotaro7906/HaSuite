﻿using HaCreator.MapEditor.Instance;
using HaCreator.Wz;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using MapleLib.WzLib.WzStructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaCreator.MapEditor.Info
{
    public class NpcInfo : MapleExtractableInfo
    {
        private string id;
        private string name;

        private WzImage LinkedImage;

        public NpcInfo(Bitmap image, System.Drawing.Point origin, string id, string name, WzObject parentObject)
            : base(image, origin, parentObject)
        {
            this.id = id;
            this.name = name;
        }

        private void ExtractPNGFromImage(WzImage image)
        {
            WzCanvasProperty npcImage = WzInfoTools.GetNpcImage(image);
            if (npcImage != null)
            {
                Image = npcImage.PngProperty.GetPNG(false);
                Origin = WzInfoTools.VectorToSystemPoint((WzVectorProperty)npcImage["origin"]);
            }
            else
            {
                Image = new Bitmap(1, 1);
                Origin = new System.Drawing.Point();
            }
        }

        public override void ParseImage()
        {
            WzStringProperty link = (WzStringProperty)((WzSubProperty)((WzImage)ParentObject)["info"])["link"];
            if (link != null)
            {
                LinkedImage = (WzImage)Program.WzManager["npc"][link.Value + ".img"];
                ExtractPNGFromImage(LinkedImage);
            }
            else
            {
                ExtractPNGFromImage((WzImage)ParentObject);
            }
        }

        public static NpcInfo Load(WzImage parentObject)
        {
            string id = /*WzInfoTools.RemoveLeadingZeros(*/WzInfoTools.RemoveExtension(parentObject.Name)/*)*/;
            return new NpcInfo(null, new System.Drawing.Point(), id, WzInfoTools.GetNpcNameById(id), parentObject);
        }

        public override BoardItem CreateInstance(Layer layer, Board board, int x, int y, int z, bool flip)
        {
            if (Image == null) ParseImage();
            return new NpcInstance(this, board, x, y, UserSettings.Npcrx0Offset, UserSettings.Npcrx1Offset, 8, null, 0, flip, false, null, null);
        }

        public BoardItem CreateInstance(Board board, int x, int y, int rx0Shift, int rx1Shift, int yShift, string limitedname, int? mobTime, MapleBool flip, MapleBool hide, int? info, int? team)
        {
            if (Image == null) ParseImage();
            return new NpcInstance(this, board, x, y, rx0Shift, rx1Shift, yShift, limitedname, mobTime, flip, hide, info, team);
        }

        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }
    }
}