using System.Collections.Generic;
using UnityEngine;

public class RenderTexturesPool
{
    private struct TextureHolder
    {
        public RenderTexture texture;
        public bool isHold;

        public TextureHolder(RenderTexture texture, bool isHold)
        {
            this.texture = texture;
            this.isHold = isHold;
        }
    }

    public static bool IsInitialized => s_isInitialized;

    private static List<TextureHolder> s_holders;
    private static bool s_isInitialized = false;

    public static RenderTexture GetTexture(RenderTextureDescriptor descriptor)
    {
        if (s_isInitialized == false)
            Init();

        for (int i = 0; i < s_holders.Count; i++)
        {
            TextureHolder holder = s_holders[i];

            if (holder.isHold) continue;

            if (DescriptorEquals(holder.texture.descriptor, descriptor))
            {
                s_holders[i] = new TextureHolder(holder.texture, true);

                return holder.texture;
            }
        }

        return CreateTexture(descriptor, true);
    }

    public static void Dispose(RenderTexture texture)
    {
        if (!s_isInitialized) return;

        for (int i = 0; i < s_holders.Count; i++)
        {
            TextureHolder holder = s_holders[i];
            if (holder.texture == texture)
            {
                s_holders[i] = new TextureHolder(texture, false);
                return;
            }
        }

        throw new System.ArgumentException("Not found texture in the pool");
    }

    public static void CleanUp()
    {
        for (int i = 0; i < s_holders.Count; i++)
        {
            TextureHolder holder = s_holders[i];

            if (holder.isHold == false)
            {
                holder.texture.Release();
                s_holders.RemoveAt(i);
            }
        }
    }

    private static void Init()
    {
        s_holders = new List<TextureHolder>();

        s_isInitialized = true;
    }

    private static bool DescriptorEquals(in RenderTextureDescriptor desc1, in RenderTextureDescriptor desc2)
    {
        return desc1.flags == desc2.flags && desc1.graphicsFormat == desc2.graphicsFormat
            && desc1.width == desc2.width && desc1.height == desc2.height
            && desc1.msaaSamples == desc2.msaaSamples && desc1.volumeDepth == desc2.volumeDepth
            && desc1.mipCount == desc2.mipCount && desc1.stencilFormat == desc2.stencilFormat
            && desc1.depthStencilFormat == desc2.depthStencilFormat
            && desc1.dimension == desc2.dimension && desc1.shadowSamplingMode == desc2.shadowSamplingMode
            && desc1.vrUsage == desc2.vrUsage && desc1.memoryless == desc2.memoryless;
    }

    public static void DestroyPool()
    {
        foreach (var item in s_holders)
        {
            item.texture.Release();
        }

        s_holders.Clear();

        s_isInitialized = false;
    }

    private static RenderTexture CreateTexture(RenderTextureDescriptor descriptor, bool isHold)
    {
        RenderTexture created = new RenderTexture(descriptor);

        var holder = new TextureHolder(created, isHold);

        s_holders.Add(holder);

        return created;
    }
}
