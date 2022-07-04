using System;

namespace GGroupp.Internal.Support.Flow.Incident.Create;

partial class AsyncPipelineExtensions
{
    internal static AsyncPipeline<TSuccess, TNextFailure> Recover<TSuccess, TFailure, TNextFailure>(
        this AsyncPipeline<TSuccess, TFailure> pipeline, Func<TFailure, Result<TSuccess, TNextFailure>> next)
        where TFailure : struct
        where TNextFailure : struct
        =>
        pipeline.Pipe(
            r => r.Recover(next));
}