#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace ImageMath {
    public sealed class TransferFunction {
        public string Identifier { get; }
        public string DisplayName { get; }
        public Func<Texture?, ColorTransformOperation> Operation { get; }

        public TransferFunction(
            string identifier,
            string displayName,
            Func<Texture?, ColorTransformOperation> operation) {

            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }

        public ColorTransformOperation CreateOperation(Texture? texture = null) => Operation(texture);

        public void Render(Texture input, RenderTexture output) => CreateOperation(input).AssignTo(output);
    }

    public static class TransferFunctionFactory {
        public static class Identifiers {
            public const string ArriLogC3 = "arri-log-c3";
            public const string ArriLogC4 = "arri-log-c4";
            public const string BlackmagicDesignFilmGen5 = "blackmagic-design-film-gen5";
            public const string CanonLog = "canon-log";
            public const string CanonLog2 = "canon-log2";
            public const string CanonLog3 = "canon-log3";
            public const string Rec709 = "rec709";
            public const string RedLog3G10 = "red-log3g10";
            public const string SonySlog3 = "sony-slog3";
            public const string SRGB = "srgb";
        }

        public static IEnumerable<TransferFunction> GetAvailableTransferFunctions(bool unpack) {
            yield return new TransferFunction(
                Identifiers.ArriLogC3,
                "ARRI LogC3",
                unpack ? TransferFunctions.UnpackArriLogC3 : TransferFunctions.PackArriLogC3);

            yield return new TransferFunction(
                Identifiers.ArriLogC4,
                "ARRI LogC4",
                unpack ? TransferFunctions.UnpackArriLogC4 : TransferFunctions.PackArriLogC4);

            yield return new TransferFunction(
                Identifiers.BlackmagicDesignFilmGen5,
                "Blackmagic Design Film Gen 5",
                unpack
                    ? TransferFunctions.UnpackBlackmagicDesignFilmGen5
                    : TransferFunctions.PackBlackmagicDesignFilmGen5);

            yield return new TransferFunction(
                Identifiers.CanonLog,
                "Canon Log",
                unpack ? TransferFunctions.UnpackCanonLog : TransferFunctions.PackCanonLog);

            yield return new TransferFunction(
                Identifiers.CanonLog2,
                "Canon Log 2",
                unpack ? TransferFunctions.UnpackCanonLog2 : TransferFunctions.PackCanonLog2);

            yield return new TransferFunction(
                Identifiers.CanonLog3,
                "Canon Log 3",
                unpack ? TransferFunctions.UnpackCanonLog3 : TransferFunctions.PackCanonLog3);

            yield return new TransferFunction(
                Identifiers.Rec709,
                "Rec.709",
                unpack ? TransferFunctions.UnpackRec709 : TransferFunctions.PackRec709);

            yield return new TransferFunction(
                Identifiers.RedLog3G10,
                "RED Log3G10",
                unpack ? TransferFunctions.UnpackRedLog3G10 : TransferFunctions.PackRedLog3G10);

            yield return new TransferFunction(
                Identifiers.SonySlog3,
                "Sony S-Log3",
                unpack ? TransferFunctions.UnpackSonySlog3 : TransferFunctions.PackSonySlog3);

            yield return new TransferFunction(
                Identifiers.SRGB,
                "sRGB",
                unpack ? TransferFunctions.UnpackSRGB : TransferFunctions.PackSRGB);
        }

        public static TransferFunction? Create(string identifier, bool unpack) {
            return GetAvailableTransferFunctions(unpack).FirstOrDefault(
                transferFunction => string.Equals(
                    transferFunction.Identifier,
                    identifier,
                    StringComparison.Ordinal));
        }
    }
}
