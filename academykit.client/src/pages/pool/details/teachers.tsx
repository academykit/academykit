import {
  Box,
  Button,
  Container,
  Group,
  Select,
  Title,
  Transition,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  useCreateTeacherPool,
  usePoolsTeacher,
} from "@utils/services/poolService";

import { TrainingTypeEnum } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import { useGetTrainers } from "@utils/services/adminService";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import TeacherCard from "./Component/TeacherCard";

const MCQTeacher = () => {
  const { t } = useTranslation();

  const form = useForm({
    initialValues: {
      email: "",
    },
    validate: {
      email: (value) => (!value ? t("trainer_email_required") : null),
    },
  });
  const slug = useParams();
  const [search, setSearch] = useState("");
  const getPoolsTeacher = usePoolsTeacher(slug.id as string);
  const createPoolTeacher = useCreateTeacherPool(slug.id as string);
  const lessonType = TrainingTypeEnum.QuestionPool;
  const { data: trainers, isLoading } = useGetTrainers(
    queryStringGenerator({ search }),
    lessonType,
    slug.id
  );
  const [showAddForm, toggleAddForm] = useToggle();

  const onSubmitForm = async ({ email }: { email: string }) => {
    try {
      await createPoolTeacher.mutateAsync({
        questionPoolIdentity: slug.id as string,
        email: email,
      });
      showNotification({ message: "Trainer added successfully." });

      toggleAddForm();
    } catch (err) {
      const error = errorType(err);
      showNotification({ message: error, color: "red" });
    }
  };
  return (
    <Container fluid>
      <Group style={{ justifyContent: "space-between", alignItems: "center" }}>
        <Title>{t("trainers")}</Title>
        <Button
          onClick={() => {
            toggleAddForm();
            form.reset();
          }}
        >
          {!showAddForm ? t("add_trainer") : t("cancel")}
        </Button>
      </Group>
      <Transition
        mounted={showAddForm}
        transition={"slide-down"}
        duration={200}
        timingFunction="ease"
      >
        {() => (
          <Box mt={10}>
            <form onSubmit={form.onSubmit(onSubmitForm)}>
              <Group style={{ alignItems: "start" }}>
                <Select
                  clearable
                  placeholder={t("enter_email_trainer") as string}
                  searchable
                  nothingFoundMessage={
                    isLoading ? "Loading..." : "No Trainers Found!"
                  }
                  data={trainers?.map((e) => e.email) ?? []}
                  onSearchChange={setSearch}
                  searchValue={search}
                  {...form.getInputProps("email")}
                />

                <Button type="submit" loading={createPoolTeacher.isLoading}>
                  {t("add")}
                </Button>
              </Group>
            </form>
          </Box>
        )}
      </Transition>
      <Box mt={20}>
        {getPoolsTeacher.data?.items.map((item) => (
          <TeacherCard teacher={item} key={item.id} slug={slug.id as string} />
        ))}
      </Box>
    </Container>
  );
};

export default MCQTeacher;
