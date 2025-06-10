using System;

using UnityEngine;

namespace ImageMath {

    public class Disposable : IDisposable {

        private readonly Action disposeAction;

        public Disposable(Action disposeAction) {
            this.disposeAction = disposeAction;
        }
        public void Dispose() {
            disposeAction();
        }
        public static Disposable<T> Create<T>(T value, Action<T> disposeAction) {
            return new Disposable<T>(value, disposeAction);
        }
    }

    public class Disposable<T> : IDisposable {
        public readonly T Value;
        private readonly Action<T> disposeAction;

        public Disposable(T value, Action<T> disposeAction) {
            Value = value;
            this.disposeAction = disposeAction;
        }

        public void Dispose() {
            disposeAction(Value);
        }

        public static implicit operator T(Disposable<T> disposable) {
            return disposable.Value;
        }
    }



    public static class DisposableExtensions {
        public static Disposable<T> ToDisposable<T>(this T value, Action<T> disposeAction) {
            return new Disposable<T>(value, disposeAction);
        }
        public static Disposable<Texture> ToDisposable(this Texture texture) {
            return new Disposable<Texture>(texture, t => {
                if (t != null) {
                    UnityEngine.Object.Destroy(t);
                }
            });
        }
        public static Disposable<RenderTexture> ToDisposable(this RenderTexture renderTexture) {
            return new Disposable<RenderTexture>(renderTexture, rt => {
                if (rt != null) {
                    UnityEngine.Object.Destroy(rt);
                }
            });
        }
    }
}



