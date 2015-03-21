﻿/* Copyright (C) 2015 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapleLib.WzLib.WzStructure.Data;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MapleLib.WzLib.WzStructure;

namespace HaCreator.MapEditor
{
    public interface INamedMisc
    {
        string Name { get; }
    }

    public class MiscDot : MapleDot
    {
        private MiscRectangle parentItem;

        public MiscDot(MiscRectangle parentItem, Board board, int x, int y)
            : base(board, x, y)
        {
            this.parentItem = parentItem;
        }

        public override Color Color
        {
            get { return UserSettings.MiscColor; }
        }

        public override Color InactiveColor
        {
            get { return MultiBoard.MiscInactiveColor; }
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Misc; }
        }

        protected override bool RemoveConnectedLines
        {
            get { return false; }
        }

        public MiscRectangle ParentRectangle { get { return parentItem; } set { parentItem = value; } }
    }

    public class MiscLine : MapleLine
    {
        public MiscLine(Board board, MapleDot firstDot, MapleDot secondDot)
            : base(board, firstDot, secondDot)
        {
        }

        public override Color Color
        {
            get { return UserSettings.MiscColor; }
        }

        public override Color InactiveColor
        {
            get { return MultiBoard.MiscInactiveColor; }
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Misc; }
        }

        public override void Remove(bool removeDots, List<UndoRedoAction> undoPipe)
        {
            
        }
    }

    public abstract class MiscRectangle : MapleRectangle, INamedMisc
    {
        public abstract string Name { get; }

        public MiscRectangle(Board board, Rectangle rect)
            : base(board, rect)
        {
            lock (board.ParentControl)
            {
                PointA = new MiscDot(this, board, rect.Left, rect.Top);
                PointB = new MiscDot(this, board, rect.Right, rect.Top);
                PointC = new MiscDot(this, board, rect.Right, rect.Bottom);
                PointD = new MiscDot(this, board, rect.Left, rect.Bottom);
                board.BoardItems.MiscItems.Add((MiscDot)PointA);
                board.BoardItems.MiscItems.Add((MiscDot)PointB);
                board.BoardItems.MiscItems.Add((MiscDot)PointC);
                board.BoardItems.MiscItems.Add((MiscDot)PointD);
                LineAB = new MiscLine(board, PointA, PointB);
                LineBC = new MiscLine(board, PointB, PointC);
                LineCD = new MiscLine(board, PointC, PointD);
                LineDA = new MiscLine(board, PointD, PointA);
                LineAB.yBind = true;
                LineBC.xBind = true;
                LineCD.yBind = true;
                LineDA.xBind = true;
            }
        }

        public override Color Color
        {
            get
            {
                return Selected ? UserSettings.ToolTipSelectedFill : UserSettings.ToolTipFill;
            }
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Misc; }
        }

        public override void Draw(SpriteBatch sprite, Color dotColor, int xShift, int yShift)
        {
            base.Draw(sprite, dotColor, xShift, yShift);
            board.ParentControl.FontEngine.DrawString(sprite, new System.Drawing.Point(X + xShift + 2, Y + yShift + 2), Color.Black, Name, Width);
        }
    }

    public class BuffZone : MiscRectangle
    {
        private int itemID;
        private int interval;
        private int duration;
        private string zoneName;

        public BuffZone(Board board, Rectangle rect, int itemID, int interval, int duration, string zoneName)
            : base(board, rect)
        {
            this.itemID = itemID;
            this.interval = interval;
            this.duration = duration;
            this.zoneName = zoneName;
        }

        public override string  Name
        {
	        get { return "BuffZone " + this.zoneName; }
        }

        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        public string ZoneName
        {
            get { return zoneName; }
            set { zoneName = value; }
        }
    }

    public class ShipObject : BoardItem, IFlippable, INamedMisc
    {
        private ObjectInfo baseInfo; //shipObj
        private bool flip;
        private int? x0;
        private int? zVal;
        private int tMove;
        private int shipKind;

        public ShipObject(ObjectInfo baseInfo, Board board, int x, int y, int? zVal, int? x0, int tMove, int shipKind, bool flip)
            : base(board, x, y, -1)
        {
            this.baseInfo = baseInfo;
            this.flip = flip;
            this.x0 = x0;
            this.zVal = zVal;
            this.tMove = tMove;
            this.shipKind = shipKind;
            if (flip)
                X -= Width - 2 * Origin.X;
        }

        public int? X0
        {
            get { return x0; }
            set { x0 = value; }
        }

        public int? zValue
        {
            get { return zVal; }
            set { zVal = value; }
        }

        public int TimeMove
        {
            get { return tMove; }
            set { tMove = value; }
        }

        public int ShipKind
        {
            get { return shipKind; }
            set { shipKind = value; }
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Misc; }
        }

        public string Name
        {
            get
            {
                return "Special: Ship";
            }
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public override Color GetColor(SelectionInfo sel, bool selected)
        {
            Color c = base.GetColor(sel, selected);
            return c;
        }

        public bool Flip
        {
            get
            {
                return flip;
            }
            set
            {
                if (flip == value) return;
                flip = value;
                int xFlipShift = Width - 2 * Origin.X;
                if (flip) X -= xFlipShift;
                else X += xFlipShift;
            }
        }

        public int UnflippedX
        {
            get
            {
                return flip ? (X + Width - 2 * Origin.X) : X;
            }
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0, 0), Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0 /*Layer.LayerNumber / 10f + Z / 1000f*/);
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override int Width
        {
            get { return baseInfo.Width; }
        }

        public override int Height
        {
            get { return baseInfo.Height; }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }
    }

    public class Healer : BoardItem, INamedMisc
    {
        private ObjectInfo baseInfo;
        public int yMin;
        public int yMax;
        public int healMin;
        public int healMax;
        public int fall;
        public int rise;

        public Healer(ObjectInfo baseInfo, Board board, int x, int yMin, int yMax, int healMin, int healMax, int fall, int rise)
            : base(board, x, (yMax + yMin) / 2, -1)
        {
            this.baseInfo = baseInfo;
            this.yMin = yMin;
            this.yMax = yMax;
            this.healMin = healMin;
            this.healMax = healMax;
            this.fall = fall;
            this.rise = rise;
        }

        public override int Y
        {
            get
            {
                return (yMax + yMin) / 2;
            }
            set
            {
                lock (board.ParentControl)
                {
                    int offs = value - Y;
                    yMax += offs;
                    yMin += offs;
                }
            }
        }

        public override void Move(int x, int y)
        {
            lock (board.ParentControl)
            {
                position.X = x;
                int offs = y - Y;
                yMax += offs;
                yMin += offs;
            }
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Misc; }
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public override Color GetColor(SelectionInfo sel, bool selected)
        {
            Color c = base.GetColor(sel, selected);
            return c;
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public override bool CheckIfLayerSelected(SelectionInfo sel)
        {
            return true;
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override int Width
        {
            get { return baseInfo.Width; }
        }

        public override int Height
        {
            get { return baseInfo.Height; }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }

        public string Name
        {
            get
            {
                return "Special: Healer";
            }
        }
    }

    public class Pulley : BoardItem, INamedMisc
    {
        private ObjectInfo baseInfo;

        public Pulley(ObjectInfo baseInfo, Board board, int x, int y)
            : base(board, x, y, -1)
        {
            this.baseInfo = baseInfo;
        }

        public override ItemTypes Type
        {
            get { return ItemTypes.Misc; }
        }

        public override MapleDrawableInfo BaseInfo
        {
            get { return baseInfo; }
        }

        public override Color GetColor(SelectionInfo sel, bool selected)
        {
            Color c = base.GetColor(sel, selected);
            return c;
        }

        public override void Draw(SpriteBatch sprite, Color color, int xShift, int yShift)
        {
            Rectangle destinationRectangle = new Rectangle((int)X + xShift - Origin.X, (int)Y + yShift - Origin.Y, Width, Height);
            sprite.Draw(baseInfo.GetTexture(sprite), destinationRectangle, null, color, 0f, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public override System.Drawing.Bitmap Image
        {
            get
            {
                return baseInfo.Image;
            }
        }

        public override int Width
        {
            get { return baseInfo.Width; }
        }

        public override int Height
        {
            get { return baseInfo.Height; }
        }

        public override System.Drawing.Point Origin
        {
            get
            {
                return baseInfo.Origin;
            }
        }

        public string Name
        {
            get
            {
                return "Special: Pulley";
            }
        }
    }

    public class Clock : MiscRectangle
    {
        public Clock(Board board, Rectangle rect)
            : base(board, rect)
        {
        }

        public override string Name
        {
            get { return "Clock"; }
        }
    }

    public class Area : MiscRectangle
    {
        string id;

        public Area(Board board, Rectangle rect, string id)
            : base(board, rect)
        {
            this.id = id;
        }

        public string Identifier
        {
            get { return id; }
            set { id = value; }
        }

        public override string Name
        {
            get { return "Area " + id; }
        }
    }

    public class SwimArea : MiscRectangle
    {
        string id;

        public SwimArea(Board board, Rectangle rect, string id)
            : base(board, rect)
        {
            this.id = id;
        }

        public string Identifier
        {
            get { return id; }
            set { id = value; }
        }

        public override string Name
        {
            get { return "SwimArea " + id; }
        }
    }
}
