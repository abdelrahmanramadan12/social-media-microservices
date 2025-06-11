using Application.Abstractions;
using Application.Events;
using Workers.Listeners;

namespace Web.ServiceCollections
{
    public static class QueueListenersCollection
    {
        public static IServiceCollection AddQueueListeners(this IServiceCollection services)
        {
            services.AddSingleton<IQueueListener<PostEvent>, PostQueueListener>();
            services.AddSingleton<IQueueListener<ProfileEvent>, ProfileQueueListener>();
            services.AddSingleton<IQueueListener<FollowEvent>, FollowQueueListener>();
            services.AddSingleton<IQueueListener<CommentEvent>, CommentQueueListener>();
            services.AddSingleton<IQueueListener<ReactEvent>, ReactQueueListener>();

            return services;
        }
    }
}
