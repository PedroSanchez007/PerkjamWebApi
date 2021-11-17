using Perkjam.API.Entities;
using System;
using System.Collections.Generic;

namespace Perkjam.API.Services
{
    public interface IGalleryRepository
    {
        IEnumerable<Image> GetImages();
        bool IsImageOwner(Guid id, string ownerId);
        Image GetImage(Guid id);
        bool ImageExists(Guid id);
        void AddImage(Image image);
        void UpdateImage(Image image);
        void DeleteImage(Image image);
        bool Save();
    }
}
