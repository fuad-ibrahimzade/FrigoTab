﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FrigoTab {

    public class Thumbnail : IDisposable {

        static Thumbnail () {
            DwmEnableComposition(DwmEnableCompositionConstants.EnableComposition);
        }

        private readonly IntPtr _thumbnail;

        public Thumbnail (WindowHandle source, WindowHandle destination, ScreenRect bounds) {
            DwmRegisterThumbnail(destination, source, out _thumbnail);

            DwmThumbnailProperties properties = new DwmThumbnailProperties {
                Flags = DwmThumbnailFlags.RectSource | DwmThumbnailFlags.RectDestination,
                Source = new ClientRect(Point.Empty, GetSourceSize()),
                Destination = bounds.ScreenToClient(destination)
            };
            DwmUpdateThumbnailProperties(_thumbnail, ref properties);
        }

        public void Dispose () {
            DwmUnregisterThumbnail(_thumbnail);
        }

        public Size GetSourceSize () {
            Size size;
            DwmQueryThumbnailSourceSize(_thumbnail, out size);
            return size;
        }

        private struct DwmThumbnailProperties {

            public DwmThumbnailFlags Flags;
            public ClientRect Destination;
            public ClientRect Source;
            public byte Opacity;
            public bool Visible;
            public bool SourceClientAreaOnly;

        }

        [Flags]
        private enum DwmThumbnailFlags {

            RectDestination = 1,
            RectSource = 2,
            Opacity = 4,
            Visible = 8,
            SourceClientAreaOnly = 16

        }

        private enum DwmEnableCompositionConstants {

            EnableComposition = 1

        }

        [DllImport ("dwmapi.dll")]
        private static extern int DwmEnableComposition (DwmEnableCompositionConstants uCompositionAction);

        [DllImport ("dwmapi.dll")]
        private static extern int DwmRegisterThumbnail (IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport ("dwmapi.dll")]
        private static extern int DwmUnregisterThumbnail (IntPtr thumb);

        [DllImport ("dwmapi.dll")]
        private static extern int DwmUpdateThumbnailProperties (IntPtr thumb, ref DwmThumbnailProperties props);

        [DllImport ("dwmapi.dll")]
        private static extern int DwmQueryThumbnailSourceSize (IntPtr thumb, out Size pSize);

    }

}
