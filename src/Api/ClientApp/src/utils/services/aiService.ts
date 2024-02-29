import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { AiModelEnum } from '@utils/enums';

interface ITrainingSuggest {
  title: string;
  description: string;
}

interface IMasterSetup {
  key: string | null;
  isActive: boolean;
  aiModel: AiModelEnum;
}

const getTrainingSuggestion = async () =>
  await httpClient.get<ITrainingSuggest>(api.ai.trainingSuggest);

export const useTrainingSuggestion = (enabled?: boolean) => {
  return useQuery([api.ai.trainingSuggest], () => getTrainingSuggestion(), {
    select: (data) => data.data,
    enabled: enabled ?? true,
  });
};

const getAiMaster = async () =>
  await httpClient.get<IMasterSetup>(api.ai.aiMasterSetup);

export const useAIMaster = () => {
  return useQuery([api.ai.aiMasterSetup], () => getAiMaster(), {
    select: (data) => data.data,
  });
};

const updateAISetup = async ({ data }: { data: IMasterSetup }) =>
  httpClient.put<IMasterSetup>(api.ai.aiMasterSetup, data);

export const useUpdateAISetup = () => {
  const queryClient = useQueryClient();
  return useMutation(['update' + api.ai.aiMasterSetup], updateAISetup, {
    onSuccess: () => {
      queryClient.invalidateQueries([api.ai.aiMasterSetup]);
    },
  });
};
