import { Button, Checkbox, Flex, NumberInput, Switch } from "@mantine/core";
import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  IUpdateShuffle,
  useGetShuffleDetails,
  useUpdateShuffle,
} from "@utils/services/courseService";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

const Shuffle = () => {
  const { t } = useTranslation();
  const params = useParams();
  const shuffleDetail = useGetShuffleDetails(
    params.id as string,
    params.lessonSlug as string
  );
  const updateShuffle = useUpdateShuffle(
    params.id as string,
    params.lessonSlug as string
  );

  const form = useForm({
    initialValues: {
      noOfQuestion: shuffleDetail.data?.noOfQuestion ?? 0,
      isShuffle: shuffleDetail.data?.isShuffle ?? false,
      showAll: shuffleDetail.data?.showAll ?? true,
    },
  });

  const handleSubmit = async (data: IUpdateShuffle) => {
    try {
      await updateShuffle.mutateAsync({
        trainingSlug: params.id as string,
        lessonSlug: params.lessonSlug as string,
        data,
      });
      showNotification({
        message: t("successful"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        title: t("error"),
        color: "red",
      });
    }
  };

  // set no of question to 0 if show all is true
  useEffect(() => {
    if (form.values.showAll) {
      form.setFieldValue("noOfQuestion", 0);
    }
  }, [form.values.showAll]);

  return (
    <>
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Flex align={"flex-end"} gap={15}>
          <Checkbox
            label={t("all")}
            labelPosition="right"
            {...form.getInputProps("showAll", { type: "checkbox" })}
          />
          <NumberInput
            min={0}
            defaultValue={0}
            label={t("no_of_question")}
            disabled={form.values.showAll}
            {...form.getInputProps("noOfQuestion")}
          />
          <Switch
            label={t("shuffle")}
            {...form.getInputProps("isShuffle", { type: "checkbox" })}
          />
          <Button type="submit">{t("submit")}</Button>
        </Flex>
      </form>
    </>
  );
};

export default Shuffle;
