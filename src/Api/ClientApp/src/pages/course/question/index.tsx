import Breadcrumb from '@components/Ui/BreadCrumb';
import {
  Anchor,
  Button,
  Grid,
  Group,
  Loader,
  Modal,
  Pagination,
  Paper,
  Select,
  Text,
  useMantineTheme,
} from '@mantine/core';
import { useDisclosure, useMediaQuery } from '@mantine/hooks';
import { showNotification } from '@mantine/notifications';
import TransferList from '@pages/course/component/TransferList';
import errorType from '@utils/services/axiosError';
import { usePools } from '@utils/services/poolService';
import {
  useAddQuestionQuestionSet,
  useQuestion,
  useQuestionSetQuestions,
} from '@utils/services/questionService';
import { api } from '@utils/services/service-api';
import { httpClient } from '@utils/services/service-axios';
import { ITag } from '@utils/services/tagService';
import { IPaginated } from '@utils/services/types';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import QuestionForm from './components/QuestionForm';

interface ISelectList {
  label: string;
  value: string;
}

export interface IQuestionListData {
  value: string;
  label: string;
  description: string;
}

const Questions = () => {
  const theme = useMantineTheme();
  const params = useParams();
  const [opened, { open, close }] = useDisclosure(false);
  const lessonSlug = params.lessonSlug && params.lessonSlug;
  const questionList = useQuestionSetQuestions(lessonSlug as string);

  const firstList: any = [];
  const secondList: any = [];
  const [activePage, setPage] = useState(1);
  const { t } = useTranslation();

  const [data, setData] = useState([firstList, secondList]);
  const [poolValue, setPoolValue] = useState<string | null>(null);
  const [tagValue, setTagValue] = useState<string | null>(null);
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.sm}px)`);
  const questionPools = usePools('');
  // const questionPoolTags = useTags('', poolValue ?? '', 1);
  const [questionPoolTags, setQuestionPoolTags] = useState<IPaginated<ITag>>(
    {} as IPaginated<ITag>
  );
  const questions = useQuestion(
    poolValue ?? '',
    `page=${activePage}&size=12&${tagValue ? `tags=${[tagValue]}` : ''}`
  );

  const callAPI = async (
    search: string,
    identity: string,
    trainingType: number
  ) => {
    try {
      const response = await httpClient.get<IPaginated<ITag>>(
        api.tags.list +
          `?${search}${identity ? `Idenitiy=${identity}` : ''}${
            trainingType ? `&TrainingType=${trainingType}` : ''
          }`
      );
      setQuestionPoolTags(response.data);
    } catch (error) {
      showNotification({
        title: t('error'),
        message: 'Something went wrong',
        color: 'red',
      });
    }
  };

  // get tags on every pool value change
  useEffect(() => {
    callAPI('', poolValue ?? '', 1);
  }, [poolValue]);

  const addQuestions = useAddQuestionQuestionSet(lessonSlug as string);
  const navigate = useNavigate();

  const poolData: ISelectList[] = [];
  const questionTag: ISelectList[] = [];

  questionPools.data?.items.map((e) => {
    poolData.push({ value: e.slug, label: e.name });
  });

  questionPoolTags?.items?.map((e) => {
    questionTag.push({ value: e.id, label: e.name });
  });

  useEffect(() => {
    if (poolValue) {
      const i: IQuestionListData[] | undefined = questions.data?.items.map(
        (e) => {
          return {
            value: e.questionPoolQuestionId,
            label: e.name,
            description: e.description,
          };
        }
      );

      if (i) {
        const difference = i?.filter(
          (x: any) => !data[1].some((e: any) => x.value === e.value)
        );
        setData([difference, data[1]]);
      }
    } else {
      setPage(1);
      setData([[], data[1]]);
    }
  }, [questions.isSuccess, poolValue, activePage, tagValue]);

  useEffect(() => {
    const i: any = questionList.data?.map((e) => {
      return {
        value: e.questionPoolQuestionId,
        label: e.name,
        description: e.description,
      };
    });
    if (i) {
      setData([data[0], i]);
    }
  }, [questionList.isSuccess]);

  const addQuestion = async () => {
    if (!data[1].length) {
      showNotification({
        message: t('question_list_cannot_empty'),
        color: 'red',
      });
      return;
    }
    try {
      const questionPoolQuestionIds: string[] = [];
      data[1].map((e: any) => {
        questionPoolQuestionIds.push(e.value);
      });
      await addQuestions.mutateAsync({
        questionPoolQuestionIds,
        identity: params.lessonSlug ?? '',
      });
      showNotification({
        message: t('add_questions_success'),
      });
      navigate(-1);
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: t('error'),
        message: err,
        color: 'red',
      });
    }
  };

  return (
    <div>
      <Modal opened={opened} onClose={close} size="auto">
        <QuestionForm closeModal={close} setTransferListData={setData} />
      </Modal>
      <Breadcrumb hide={3} />
      <Paper p={10} withBorder>
        <Grid mb={10}>
          <Grid.Col span={matches ? 3 : 6}>
            <Select
              size="md"
              placeholder={t('pick_one') as string}
              label={
                <>
                  <Text>{t('mcq_pools')}</Text>
                  <Text size={'0.810rem'} c="#909296">
                    {t('question_pool_created')}
                    <Anchor href="/pools">
                      {t('question_pool_href')}
                    </Anchor>{' '}
                    {t('section')}
                    <Anchor href="https://docs.academykit.co/app-documentation/training/questions">
                      {t('learn_more')}
                    </Anchor>
                  </Text>
                </>
              }
              searchable
              clearable
              allowDeselect
              nothingFoundMessage={t('no_options')}
              maxDropdownHeight={280}
              data={poolData}
              onChange={(e) => {
                setPoolValue(e);
                setPage(1);
              }}
            />
          </Grid.Col>
          <Grid.Col mt={21} span={matches ? 3 : 6}>
            {poolValue && (
              <Select
                size="md"
                label="Tags"
                placeholder="Pick one tag"
                searchable
                clearable
                allowDeselect
                nothingFoundMessage="No Tags found!"
                maxDropdownHeight={280}
                data={questionTag}
                onChange={setTagValue}
              />
            )}
          </Grid.Col>
        </Grid>

        {questions.fetchStatus !== 'idle' && questions.isLoading ? (
          <Loader />
        ) : (
          <>
            <TransferList data={data} setData={setData} openModal={open} />

            {questions.data && questions.data.totalPage > 1 && (
              <Pagination
                mt={10}
                value={activePage}
                onChange={setPage}
                total={questions.data?.totalPage ?? 1}
              />
            )}
          </>
        )}
        <Group mt={30}>
          <Button onClick={addQuestion}>{t('submit')}</Button>
          <Button
            variant="outline"
            onClick={() => {
              navigate(-1);
            }}
          >
            {t('cancel')}
          </Button>
        </Group>
      </Paper>
    </div>
  );
};

export default Questions;
