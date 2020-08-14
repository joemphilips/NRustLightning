using System;
using System.Linq;
using NBitcoin;

namespace NRustLightning.Infrastructure.Models
{
    public class BlockHeaderData : IBitcoinSerializable, IEquatable<BlockHeaderData>
    {
        private uint _height;
        private BlockHeader _header;
        public Target ChainWork => this.Header.Bits;

        public uint Height => _height;

        public BlockHeader Header => _header;

        private BlockHeaderData() {}
        public BlockHeaderData(uint height, BlockHeader header)
        {
            _height = height;
            _header = header ?? throw new ArgumentNullException(nameof(header));
        }
        public void ReadWrite(BitcoinStream stream)
        {
            stream.ReadWrite(ref _height);
            stream.ReadWrite(ref _header);
        }

        public BlockHeaderData Clone()
        {
            var t = new BlockHeaderData();
            t.FromBytes(this.ToBytes());
            return t;
        }

        public bool Equals(BlockHeaderData? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _height == other._height && _header.ToBytes().SequenceEqual(other._header.ToBytes());
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BlockHeaderData) obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable twice NonReadonlyMemberInGetHashCode
            return HashCode.Combine(_height, _header);
        }
    }
}