using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CocosSharp
{
    public class CCGrabber
    {
        private RenderTarget2D m_pOldRenderTarget;

        public void Grab(CCTexture2D pTexture)
        {
			CCDrawManager.CreateRenderTarget(pTexture, CCRenderTargetUsage.DiscardContents);
        }

        public void BeforeRender(CCTexture2D pTexture)
        {
            m_pOldRenderTarget = CCDrawManager.GetRenderTarget();
            CCDrawManager.SetRenderTarget(pTexture);
            CCDrawManager.Clear(CCColor4B.Transparent);
        }

        public void AfterRender(CCTexture2D pTexture)
        {
            CCDrawManager.SetRenderTarget(m_pOldRenderTarget);
        }
    }
}