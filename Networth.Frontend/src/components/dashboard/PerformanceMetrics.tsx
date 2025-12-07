
export function PerformanceMetrics() {
    return (
        <div className="relative min-h-[200px] flex items-center justify-center border border-dashed border-slate-700 rounded-lg p-6">
            <div className="text-center space-y-4 opacity-50">
                <h3 className="text-lg font-semibold text-white">
                    Performance Metrics
                </h3>
                <div className="space-y-2">
                    <div className="h-4 bg-slate-700 rounded w-full"></div>
                    <div className="h-4 bg-slate-700 rounded w-full"></div>
                    <div className="h-4 bg-slate-700 rounded w-full"></div>
                </div>
            </div>
            <div className="absolute inset-0 flex items-center justify-center bg-black/20 backdrop-blur-[1px] rounded-lg">
                <span className="bg-black/50 px-4 py-2 rounded text-white font-semibold">
                    To be implemented
                </span>
            </div>
        </div>
    );
}
