﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class TmsTileGrid : ITileGrid
    {
        protected double[] _resolutions;
        protected Extent _extent;
        protected Coordinate _origin;
        protected Size _tileSize;
        protected List<Extent> _tileRanges = null;
        protected double _totalTile = 0;

        public TmsTileGrid(double[] resolutions, Extent extent, Coordinate origin,Size tileSize)
        {
            this._resolutions = resolutions;
            this._extent = extent;
            this._origin = origin;
            this._tileSize = tileSize;
            this._tileRanges = CalculateTileRanges();
        }

        protected  virtual List<Extent> CalculateTileRanges()
        {
            List<Extent> result = new List<Extent>();
            for (int i = 0, len = _resolutions.Length; i < len; i++)
            {
                var leftBottomTileCoord = GetTileCoordByXYAndZoom(i, this._extent.GetLeftBottom(), false);
                var rightTopTileCoord = GetTileCoordByXYAndZoom(i, this._extent.GetTopRight(), true);
                result.Add(new Extent(leftBottomTileCoord, rightTopTileCoord));
                _totalTile += (rightTopTileCoord.X - leftBottomTileCoord.X +1 ) * (rightTopTileCoord.Y - leftBottomTileCoord.Y +1);
            }
            return result;
        }

        protected virtual Coordinate GetTileCoordByXYAndZoom(int zoom, Coordinate coord, bool reverseIntersectionPolicy)
        {
            var adjustX = reverseIntersectionPolicy ? 0.5 : 0.0;
            var adjustY = reverseIntersectionPolicy ? 0.0 : 0.5;
            var resolution = this._resolutions[zoom];
            var xFromOrigin = Math.Floor((coord.X -this._origin.X) / resolution + adjustX);
            var yFromOrigin = Math.Floor((coord.Y - this._origin.Y) / resolution + adjustY);
            var tileCoordX = xFromOrigin / this._tileSize.Width;
            var tileCoordY = yFromOrigin / this._tileSize.Height;
            if (reverseIntersectionPolicy)
            {
                tileCoordX = Math.Ceiling(tileCoordX) - 1;
                tileCoordY = Math.Ceiling(tileCoordY) - 1;
            }
            else
            {
                tileCoordX = Math.Floor(tileCoordX);
                tileCoordY = Math.Floor(tileCoordY);
            }
            return new Coordinate(tileCoordX, tileCoordY);
        }

        public Extent GetTileExtent(TileCoord tileCoord)
        {
            var origin = this._origin;
            var resolution = this._resolutions[tileCoord.Zoom];
            var tileSize = this._tileSize;
            var minX = origin.X+ tileCoord.X * tileSize.Width * resolution;
            var minY = origin.Y+ tileCoord.Y * tileSize.Height * resolution;
            var maxX = minX + tileSize.Width * resolution;
            var maxY = minY + tileSize.Height* resolution;
            return new Extent(new double[] { minX, minY, maxX, maxY });
        }

        public List<Extent> TileRanges
        {
            get
            {
                return _tileRanges;
            }
        }

        public double TotalTile
        {
            get
            {
                return _totalTile;
            }
        }

        public Size TileSize
        {
            get
            {
                return _tileSize;
            }
        } 
    }
}
