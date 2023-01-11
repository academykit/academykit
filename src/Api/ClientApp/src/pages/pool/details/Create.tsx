import TextEditor from "@components/Ui/TextEditor";
import {
  Box,
  Button,
  Card,
  Checkbox,
  Container,
  Group,
  Loader,
  MultiSelect,
  Select,
  Text,
  TextInput,
  UnstyledButton,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconPlus, IconTrash } from "@tabler/icons";
import { QuestionType, ReadableEnum } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import errorType from "@utils/services/axiosError";
import {
  IAddQuestionType,
  useAddQuestion,
} from "@utils/services/questionService";
import { useAddTag, useTags } from "@utils/services/tagService";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import * as Yup from "yup";
const [FormProvider, useFormContext, useForm] =
  createFormContext<IAddQuestionType>();

const schema = Yup.object().shape({
  name: Yup.string().required("Title of Question is required!"),
  type: Yup.string().required("Question type is required!").nullable(),

  answers: Yup.array()
    .when(["type"], {
      is: QuestionType.MultipleChoice.toString(),
      then: Yup.array()
        .min(1, "Options should be more than one!")
        .test(
          "test",
          "On Multiple Choice at least one option should be selected! ",
          function (value: any) {
            const a = value?.filter((x: any) => x.isCorrect).length > 0;
            return a;
          }
        )
        .of(
          Yup.object().shape({
            option: Yup.string().trim().required("Options is required!"),
          })
        ),
    })
    .when(["type"], {
      is: QuestionType.SingleChoice.toString(),
      then: Yup.array()
        .test(
          "test",
          "On Single choice, only one option should be selected! ",
          function (value: any) {
            const length: number =
              value && value.filter((e: any) => e.isCorrect).length;
            return length === 1;
          }
        )
        .of(
          Yup.object().shape({
            option: Yup.string().trim().required("Options is required!"),
          })
        ),
    }),
});
const Create = () => {
  const navigate = useNavigate();

  const form = useForm({
    initialValues: {
      name: "",
      description: "",
      hints: "",
      tags: [],
      type: "",
      answers: [{ option: "", isCorrect: false }],
    },
    validate: yupResolver(schema),
  });

  const fieldSize = "md";
  const getQuestionType = () => {
    const dropdownValue = Object.entries(QuestionType)
      .splice(0, Object.entries(QuestionType).length / 2)
      .map(([key, value]) => {
        return {
          value: key,
          label:
            ReadableEnum[value as keyof typeof ReadableEnum] ??
            value.toString(),
        };
      })
      .filter((x) => x.value !== QuestionType.Subjective.toString());

    return dropdownValue;
  };

  const { id } = useParams();
  const addQuestion = useAddQuestion(id as string, "");
  const { mutate, data: addTagData, isSuccess } = useAddTag();
  const [isReset, setIsReset] = useState(false);
  const onSubmit = async (data: IAddQuestionType) => {
    try {
      await addQuestion.mutateAsync({ poolId: id as string, data });
      const tags = form.values.tags;
      form.reset();
      if (!isReset) {
        navigate(-1);
      }
      form.setFieldValue("tags", tags);
      showNotification({
        message: "Question has been created successfully.",
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  const [searchParams, setSearchParams] = useState("");
  const [tagsList, setTagsList] = useState<{ value: string; label: string }[]>(
    []
  );
  const tags = useTags(
    queryStringGenerator({
      search: searchParams,
      size: 10000,
    })
  );

  useEffect(() => {
    if (tags.isSuccess && tags.isFetched) {
      setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
    }
  }, [tags.isSuccess]);

  useEffect(() => {
    if (isSuccess) {
      setTagsList([
        ...tagsList,
        { label: addTagData.data.name, value: addTagData.data.id },
      ]);
      form.setFieldValue("tags", [...form.values.tags, addTagData?.data?.id]);
    }
  }, [isSuccess]);

  return (
    <Container fluid>
      <FormProvider form={form}>
        <Card mt={20}>
          <form onSubmit={form.onSubmit(onSubmit)}>
            <TextInput
              size={"lg"}
              withAsterisk
              label="Title for question"
              placeholder="Enter Title of Question"
              {...form.getInputProps("name")}
            ></TextInput>
            <Box mt={20}>
              <Text size={"lg"}>Description</Text>
              <TextEditor formContext={useFormContext} />
            </Box>

            {tags.isSuccess ? (
              <MultiSelect
                mt={15}
                searchable
                labelProps="name"
                creatable
                sx={{ maxWidth: "500px" }}
                data={tagsList}
                value={[]}
                {...form.getInputProps("tags")}
                getCreateLabel={(query) => `+ Create ${query}`}
                onCreate={(query) => {
                  mutate(query);
                }}
                size={"lg"}
                label="Tags"
                placeholder="Please select Tags."
              />
            ) : (
              <Loader />
            )}

            <Box mt={20}>
              <Text size={"lg"}>Hint</Text>
              <TextEditor label="hints" formContext={useFormContext} />
            </Box>

            <Select
              mt={20}
              placeholder={"Please select question type"}
              size={"lg"}
              allowDeselect
              withAsterisk
              label="Question Type"
              {...form.getInputProps("type")}
              data={getQuestionType()}
            ></Select>
            {(form.values.type === QuestionType.MultipleChoice.toString() ||
              form.values.type === QuestionType.SingleChoice.toString()) && (
              <Box>
                <Text mt={20}>Options</Text>
                {form.values.answers.map((x, i) => (
                  <Group key={i} mb={30}>
                    <Checkbox
                      {...form.getInputProps(`answers.${i}.isCorrect`)}
                      name=""
                    ></Checkbox>
                    <TextEditor
                      label={`answers.${i}.option`}
                      formContext={useFormContext}
                    ></TextEditor>
                    <UnstyledButton
                      onClick={() => {
                        form.insertListItem(
                          "answers",
                          {
                            option: "",
                            isCorrect: false,
                          },
                          i + 1
                        );
                      }}
                    >
                      <IconPlus color="green" />
                    </UnstyledButton>
                    {form.values.answers.length > 1 && (
                      <UnstyledButton
                        onClick={() => {
                          form.removeListItem("answers", i);
                        }}
                      >
                        <IconTrash color="red" />
                      </UnstyledButton>
                    )}
                    {typeof form.errors[`answers.${i}.option`] === "string" && (
                      <span style={{ color: "red" }}>
                        {form.errors[`answers.${i}.option`]}
                      </span>
                    )}
                  </Group>
                ))}
                {typeof form.errors[`answers`] === "string" && (
                  <span style={{ color: "red" }}>{form.errors[`answers`]}</span>
                )}
              </Box>
            )}
            <Group mt={20}>
              <Button type="submit" onClick={() => setIsReset(false)}>
                Save
              </Button>
              <Button type="submit" onClick={() => setIsReset(true)}>
                Save and add more
              </Button>
            </Group>
          </form>
        </Card>
      </FormProvider>
    </Container>
  );
};

export default Create;
