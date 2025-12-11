namespace Compress
{
    public class unas_Huffman
    {
        private static uint ID = 0x48485546;  //HHUF

        public class Node
        {
            public int Symbol;
            public int Weight;
            public Node Left;
            public Node Right;

            public bool IsLeaf => Left == null && Right == null;
        }

        private Node m_treeTop;

        public void MakeTree(ushort[] weights)
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] > 0)
                {
                    nodes.Add(new Node
                    {
                        Symbol = i,
                        Weight = weights[i]
                    });
                }
            }

            while(nodes.Count > 1)
            {
                nodes.Sort((a, b) =>
                {
                    int w = a.Weight.CompareTo(b.Weight);
                    if (w != 0) return w;
                    return a.Symbol.CompareTo(b.Symbol);
                });

                Node left = nodes[0];
                Node right = nodes[1];
                nodes.RemoveRange(0, 2);

                Node parent = new Node
                {
                    Symbol = -1,
                    Weight = left.Weight + right.Weight,
                    Left = left,
                    Right = right
                };

                nodes.Add(parent);
            }

            if (nodes.Count > 0)
                m_treeTop = nodes[0];
        }

        public int ReadSymbol(UnasBitStream stream)
        {
            Node current = m_treeTop;
            while(!current.IsLeaf)
            {
                int bit = stream.ReadBit();
                if (bit == -1) return -1;

                if (bit == 0) current = current.Left;
                else current = current.Right;
            }

            return current.Symbol;
        }

    }
}
