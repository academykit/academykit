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
  return useQuery({
    queryKey: [api.ai.trainingSuggest],
    queryFn: () => getTrainingSuggestion(),
    select: (data) => data.data,
    enabled: enabled ?? true
  });
};

const getAiMaster = async () =>
  await httpClient.get<IMasterSetup>(api.ai.aiMasterSetup);

export const useAIMaster = () => {
  return useQuery({
    queryKey: [api.ai.aiMasterSetup],
    queryFn: () => getAiMaster(),
    select: (data) => data.data
  });
};

const updateAISetup = async ({ data }: { data: IMasterSetup }) =>
  httpClient.put<IMasterSetup>(api.ai.aiMasterSetup, data);

export const useUpdateAISetup = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['update' + api.ai.aiMasterSetup],
    mutationFn: updateAISetup,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [api.ai.aiMasterSetup]
      });
    }
  });
};
