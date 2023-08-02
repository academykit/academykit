import Breadcrumb from '@components/Ui/BreadCrumb';
import { useEffect, useState } from 'react';
import {
  Checkbox,
  Group,
  Text,
  Select,
  TransferList,
  TransferListData,
  Grid,
  createStyles,
  Button,
  Paper,
  Pagination,
  Loader,
  TransferListItemComponent,
  TransferListItemComponentProps,
  Anchor,
} from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import { usePools } from '@utils/services/poolService';
import { useTags } from '@utils/services/tagService';
import {
  useAddQuestionQuestionSet,
  useQuestion,
  useQuestionSetQuestions,
} from '@utils/services/questionService';
import { showNotification } from '@mantine/notifications';
import { useNavigate, useParams } from 'react-router-dom';
import errorType from '@utils/services/axiosError';
import { useTranslation } from 'react-i18next';
import TextViewer from '@components/Ui/RichTextViewer';

interface ISelectList {
  label: string;
  value: string;
}

interface IQuestionListData {
  value: string;
  label: string;
  description: string;
}

const useStyle = createStyles({});

const ItemComponent: TransferListItemComponent = ({
  data,
  selected,
}: TransferListItemComponentProps) => (
  <Group noWrap>
    <Checkbox
      checked={selected}
      onChange={() => {}}
      tabIndex={-1}
      sx={{ pointerEvents: 'none' }}
    />
    <div style={{ flex: 1 }}>
      <Text size="sm" weight={500}>
        {data.label}
      </Text>
      {data && data?.description !== null && (
        <Text lineClamp={3} sx={{ overflow: 'hidden' }}>
          <TextViewer
            content={data?.description}
            sx={{
              wordBreak: 'break-all',
            }}
          />
        </Text>
      )}
    </div>
  </Group>
);

const Questions = () => {
  const { theme } = useStyle();
  const params = useParams();

  const lessonSlug = params.lessonSlug && params.lessonSlug;
  const questionList = useQuestionSetQuestions(lessonSlug as string);

  const firstList: any = [];
  const secondList: any = [];
  const [activePage, setPage] = useState(1);
  const { t } = useTranslation();

  const [data, setData] = useState<TransferListData>([firstList, secondList]);
  const [poolValue, setPoolValue] = useState<string | null>(null);
  const [tagValue, setTagValue] = useState<string | null>(null);
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.sm}px)`);
  const questionPools = usePools('');
  const questionPoolTags = useTags('', poolValue ?? '', 1);
  const questions = useQuestion(
    poolValue ?? '',
    `page=${activePage}&size=12&${tagValue ? `tags=${[tagValue]}` : ''}`
  );

  const addQuestions = useAddQuestionQuestionSet(lessonSlug as string);
  const navigate = useNavigate();

  const poolData: ISelectList[] = [];
  const questionTag: ISelectList[] = [];

  questionPools.data?.items.map((e) => {
    poolData.push({ value: e.slug, label: e.name });
  });

  questionPoolTags.data?.items.map((e) => {
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
      data[1].map((e) => {
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
                  <Text size={'0.810rem'} color="#909296">
                    {t('question_pool_created')}
                    <Anchor href="/pools">
                      {t('question_pool_href')}
                    </Anchor>{' '}
                    {t('section')}
                    <Anchor href="https://vuriloapp.tawk.help/category/vurilo-lms">
                      {t('learn_more')}
                    </Anchor>
                  </Text>
                </>
              }
              searchable
              clearable
              allowDeselect
              nothingFound={t('no_options')}
              maxDropdownHeight={280}
              data={poolData}
              onChange={(e) => {
                setPoolValue(e);
                setPage(1);
              }}
            />
          </Grid.Col>

          {poolValue && (
            <Grid.Col mt={21} span={matches ? 3 : 6}>
              <Select
                size="md"
                label="Tags"
                placeholder="Pick one tag"
                searchable
                clearable
                allowDeselect
                nothingFound="No Tags found!"
                maxDropdownHeight={280}
                data={questionTag}
                onChange={setTagValue}
              />
            </Grid.Col>
          )}
        </Grid>

        {questions.fetchStatus !== 'idle' && questions.isLoading ? (
          <Loader />
        ) : (
          <>
            <TransferList
              value={data}
              onChange={setData}
              searchPlaceholder={t('search_for_questions') as string}
              nothingFound={
                questionList.isLoading ? <Loader /> : t('no_question_found')
              }
              titles={[
                t('questions_list'),
                `${t('selected_questions')} (${data[1].length})`,
              ]}
              listHeight={600}
              breakpoint="sm"
              itemComponent={ItemComponent}
              sx={{ height: '85%' }}
            />

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
        <Group position="left" mt={30}>
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
