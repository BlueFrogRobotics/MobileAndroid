using System.Collections;
using System.Collections.Generic;

public class TrieNode<T> where T : new()
{
    public Hashtable Sons { get; }

    public T Content { get; set; }

    internal TrieNode()
    {
        Content = default(T);
        Sons = new Hashtable();
    }
}

public class Trie<T> where T : new()
{
    private readonly TrieNode<T> mRoot;

    public Trie()
    {
        mRoot = new TrieNode<T>();
    }

    public void Add(string iKey, T iValue)
    {
        TrieNode<T> lCurrentNode = mRoot;
        int lKeyLength = iKey.Length;

        for (int i = 0; i < lKeyLength; ++i) {
            TrieNode<T> lChildNode = (TrieNode<T>)lCurrentNode.Sons[iKey[i]];
            if (lChildNode != null)
                lCurrentNode = lChildNode;
            else {
                TrieNode<T> lNewNode = new TrieNode<T>();
                lCurrentNode.Sons.Add(iKey[i], lNewNode);
                lCurrentNode = lNewNode;
            }
        }

        lCurrentNode.Content = iValue;
    }

    public T Find(string iKey)
    {
        TrieNode<T> lCurrentNode = mRoot;
        int lKeyLength = iKey.Length;

        for (int i = 0; i < lKeyLength; ++i) {
            TrieNode<T> lChildNode = (TrieNode<T>)lCurrentNode.Sons[iKey[i]];
            if (lChildNode == null)
                return default(T);
            lCurrentNode = lChildNode;
        }

        return lCurrentNode.Content;
    }
}
