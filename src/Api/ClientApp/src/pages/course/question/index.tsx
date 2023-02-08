import Breadcrumb from "@components/Ui/BreadCrumb";
import { useEffect, useState } from "react";
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
} from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import { usePools } from "@utils/services/poolService";
import { useTags } from "@utils/services/tagService";
import {
  useAddQuestionQuestionSet,
  useQuestion,
  useQuestionSetQuestions,
} from "@utils/services/questionService";
import { showNotification } from "@mantine/notifications";
import { useNavigate, useParams } from "react-router-dom";
import errorType from "@utils/services/axiosError";
import RichTextEditor from "@mantine/rte";

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

const ItemComponent = ({
  data,
  selected,
}: {
  data: IQuestionListData;
  selected: boolean;
}) => (
  <Group noWrap>
    <Checkbox
      checked={selected}
      onChange={() => {}}
      tabIndex={-1}
      sx={{ pointerEvents: "none" }}
    />
    <div style={{ flex: 1 }}>
      <Text size="sm" weight={500}>
        {data.label}
      </Text>
      {data && data?.description !== null && (
        <Text lineClamp={3} sx={{ overflow: "hidden" }}>
          <RichTextEditor
            value={data?.description}
            readOnly
            sx={{
              wordBreak: "break-all",
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

  const [data, setData] = useState<TransferListData>([firstList, secondList]);
  const [poolValue, setPoolValue] = useState<string | null>(null);
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.sm}px)`);
  const questionPools = usePools("");
  const questionPoolTags = useTags("");
  const questions = useQuestion(poolValue ?? "", "", 1000);
  const addQuestions = useAddQuestionQuestionSet(lessonSlug as string);
  const navigate = useNavigate();

  let poolData: ISelectList[] = [];
  let questionTag: ISelectList[] = [];
  questionPools.data?.items.map((e, i) => {
    poolData.push({ value: e.slug, label: e.name });
  });
  questionPoolTags.data?.items.map((e, i) => {
    questionTag.push({ value: e.slug, label: e.name });
  });

  useEffect(() => {
    if (poolValue) {
      const i: IQuestionListData[] | undefined = questions.data?.items.map(
        (e, i) => {
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
      setData([[], data[1]]);
    }
  }, [questions.isSuccess, poolValue]);

  useEffect(() => {
    const i: any = questionList.data?.map((e, i) => {
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
        message: "Question List cannot be empty!",
        color: "red",
      });
      return;
    }
    try {
      const questionPoolQuestionIds: string[] = [];
      data[1].map((e, i) => {
        questionPoolQuestionIds.push(e.value);
      });
      await addQuestions.mutateAsync({
        questionPoolQuestionIds,
        identity: params.lessonSlug ?? "",
      });
      showNotification({
        message: "Questions added successfully!",
      });
      navigate(-1);
    } catch (error) {
      const err = errorType(error);

      showNotification({
        title: "Error",
        message: err,
        color: "red",
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
              label="Question Pool"
              placeholder="Pick one"
              searchable
              clearable
              allowDeselect
              nothingFound="No options"
              maxDropdownHeight={280}
              data={poolData}
              onChange={setPoolValue}
            />
          </Grid.Col>
        </Grid>

        <TransferList
          value={data}
          onChange={setData}
          searchPlaceholder="Search for questions"
          nothingFound={"No Questions Found!"}
          titles={["Questions List", `Selected Questions (${data[1].length})`]}
          listHeight={600}
          breakpoint="sm"
          //@ts-ignore
          itemComponent={ItemComponent}
          sx={{ height: "85%" }}
        />
        <Group position="left" mt={10}>
          <Button onClick={addQuestion}>Submit</Button>
          <Button
            variant="outline"
            onClick={() => {
              navigate(-1);
            }}
          >
            Cancel
          </Button>
        </Group>
      </Paper>
    </div>
  );
};

export default Questions;
