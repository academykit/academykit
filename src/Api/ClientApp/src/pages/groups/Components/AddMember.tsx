import { Box, Button, Loader, MultiSelect } from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import {
  useAddGroupMember,
  useGroupNotMember,
} from "@utils/services/groupService";
import { useEffect, useState, useRef } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import * as Yup from "yup";
import useFormErrorHooks from "@hooks/useFormErrorHooks";

interface IAddMember {
  email: string[];
}

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    email: Yup.array().min(1, t("one_email_required") as string),
  });
};
const AddMember = ({
  onCancel,
  searchParams,
}: {
  onCancel: () => void;
  searchParams: string;
}) => {
  const [search, setSearch] = useState("");
  const { t } = useTranslation();

  const { id } = useParams();
  const { mutateAsync, isLoading } = useAddGroupMember(
    id as string,
    searchParams
  );

  const form = useForm<IAddMember>({
    initialValues: {
      email: [],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);
  const ref = useRef<HTMLInputElement>(null);
  const getNotMemberList = useGroupNotMember(id as string, `search=${search}`);

  useEffect(() => {
    ref.current?.focus();
    if (getNotMemberList.isSuccess) {
      const t = getNotMemberList.data?.items?.map((x) => {
        return {
          label: x.email,
          value: x.email,
        };
      });
      const mergedData = [...data, ...t];
      setData([
        ...new Map(mergedData.map((item) => [item["label"], item])).values(),
      ]);
    }
  }, [getNotMemberList.isSuccess]);

  const [data, setData] = useState<{ label: string; value: string }[]>([]);
  const onSubmitForm = async (email: string[]) => {
    try {
      const response: any = await mutateAsync({
        id: id as string,
        data: { emails: email },
      });
      if (response?.data?.httpStatusCode === 206) {
        return showNotification({
          message: response?.data?.message,
        });
      }
      showNotification({
        message: response.data?.message ?? t("add_group_member_success"),
      });
      onCancel();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };
  return (
    <Box sx={{ maxWidth: "500px" }}>
      <form onSubmit={form.onSubmit(({ email }) => onSubmitForm(email))}>
        <MultiSelect
          placeholder={t("email_address") as string}
          ref={ref}
          searchable
          data={data}
          mb={10}
          label={t("email_address")}
          withAsterisk
          name="email"
          size="md"
          nothingFound={
            getNotMemberList.isLoading ? (
              <Loader />
            ) : (
              <Box>{t("User Not found")}</Box>
            )
          }
          getCreateLabel={(query) => `+ Create ${query}`}
          onSearchChange={(d) => {
            setSearch(d);
          }}
          {...form.getInputProps("email")}
        />

        <Button loading={isLoading} mr={10} type="submit" size="md">
          {t("submit")}
        </Button>
        <Button
          variant="outline"
          disabled={isLoading}
          type="reset"
          onClick={(e: any) => onCancel()}
          size={"md"}
        >
          {t("cancel")}
        </Button>
      </form>
    </Box>
  );
};

export default AddMember;
