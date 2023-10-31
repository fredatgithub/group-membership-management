// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Contracts;
using Repositories.EntityFramework.Contexts;

namespace Repositories.EntityFramework
{
    public class NotificationTypesRepository : INotificationTypesRepository
    {
        private readonly GMMContext _writeContext;
        private readonly GMMReadContext _readContext;

        public NotificationTypesRepository(GMMContext writeContext, GMMReadContext readContext)
        {
            _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));
            _readContext = readContext ?? throw new ArgumentNullException(nameof(readContext));
        }

        public async Task<int?> GetNotificationTypeIdByNotificationTypeName(string notificationName)
        {
            var notificationTypeId = await _readContext.NotificationTypes
                .FirstOrDefaultAsync(e => e.Name == notificationName);
                
            return notificationTypeId?.Id;
        }

    }
}