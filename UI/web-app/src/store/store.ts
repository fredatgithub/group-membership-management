// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { configureStore } from '@reduxjs/toolkit';
import jobsReducer from './jobs.slice';
import accountReducer from './account.slice'

export const store = configureStore({
  reducer: {
    account: accountReducer,
    jobs: jobsReducer
  }
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
// Inferred type: { jobs: userState }
export type AppDispatch = typeof store.dispatch;