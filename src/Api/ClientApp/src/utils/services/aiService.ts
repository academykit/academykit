import { useQuery } from '@tanstack/react-query';
import { api } from './service-api';
import { httpClient } from './service-axios';

interface ITrainingSuggest {
  title: string;
  description: string;
}

const getTrainingSuggestion = async () =>
  await httpClient.get<ITrainingSuggest>(api.ai.trainingSuggest);

export const useTrainingSuggestion = (enabled?: boolean) => {
  return useQuery([api.ai.trainingSuggest], () => getTrainingSuggestion(), {
    select: (data) => data.data,
    enabled: enabled ?? true,
  });
};
