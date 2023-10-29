import { createAsyncThunk } from '@reduxjs/toolkit';
import { config } from '../authConfig';
import { OnboardingStatus } from '../models/GroupOnboardingStatus';
import { Destination } from '../models/Destination';
import { ThunkConfig } from './store';
import { TokenType } from '../auth';

export class OdataQueryOptions {
  pageSize?: number;
  itemsToSkip?: number;
  filter?: String;
  orderBy?: String;
}

export const searchDestinations = createAsyncThunk<Destination[], string, ThunkConfig>(
  'destinations/searchDestinations',
  async (query: string, { extra }) => {
    const { authenticationService } = extra;
    const token = await authenticationService.getTokenAsync(TokenType.GMM);
    const headers = new Headers();
    const bearer = `Bearer ${token}`;
    headers.append('Authorization', bearer);

    const options = {
      method: 'GET',
      headers,
    };

    try {
      const response = await fetch(`${config.searchDestinations}/${encodeURIComponent(query)}`, options).then(
        async (response) => await response.json()
      );

      const payload: Destination[] = response;
      return payload;
    } catch (error) {
      throw new Error('Failed to fetch destination data!');
    }
  }
);

export const getGroupOnboardingStatus = createAsyncThunk<OnboardingStatus, string, ThunkConfig>(
  'groups/getGroupOnboardingStatus',
  async (groupId: string, { extra }) => {
    const { authenticationService } = extra;
    const token = await authenticationService.getTokenAsync(TokenType.GMM);
    const headers = new Headers();
    const bearer = `Bearer ${token}`;
    headers.append('Authorization', bearer);

    const requestOptions = {
      method: 'GET',
      headers: headers,
    };

    try {
      const response = await fetch(`${config.getGroupOnboardingStatus(groupId)}`, requestOptions);
      if (!response.ok) {
        throw new Error('Failed to fetch group onboarding status!');
      }
      const data: OnboardingStatus = await response.json();
      return data;
    } catch (error) {
      throw new Error('Failed to fetch group onboarding status!');
    }
  }
);

export const getGroupEndpoints = createAsyncThunk<string[], string, ThunkConfig>(
  'groupEndpoints',
  async (groupId: string, { extra }) => {
    const { authenticationService } = extra;
    const token = await authenticationService.getTokenAsync(TokenType.GMM);
    const headers = new Headers();
    const bearer = `Bearer ${token}`;
    headers.append('Authorization', bearer);

    const options = {
      method: 'GET',
      headers,
    };

    try {
      const response = await fetch(`${config.getGroupEndpoints(groupId)}`, options).then(
        async (response) => await response.json()
      );
      const payload: string[] = response;
      return payload;
    } catch (error) {
      throw new Error('Failed to fetch destination data!');
    }
  }
);
