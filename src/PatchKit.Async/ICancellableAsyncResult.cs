using System;

namespace PatchKit
{
	public interface ICancellableAsyncResult : IAsyncResult
	{
		/// <summary>
		/// <c>True</c> if operation has completed but it was cancelled. Otherwise <c>false</c>.
		/// </summary>
		/// <remarks>
		/// Check this property only if <see cref="IAsyncResult.IsCompleted"/> is set to true - if operation is not completed then the value will be always <c>false</c>.
		/// </remarks>
		bool IsCancelled { get; }

		/// <summary>
		/// Determines whether operation can be cancelled in it's current state.
		/// </summary>
		bool CanBeCancelled { get; }

		/// <summary>
		/// Cancels the async operation.
		/// </summary>
		/// <returns><c>True</c> if cancellation has been requested correctly. Otherwise <c>false</c>.</returns>
		bool Cancel();
	}
}
