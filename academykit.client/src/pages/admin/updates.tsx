import { Anchor, Button, Group, Text, Title } from "@mantine/core";
import { IconExternalLink } from "@tabler/icons-react";
import { useImageVersions } from "@utils/services/updateService";
import { useTranslation } from "react-i18next";

const Updates = () => {
  const { t } = useTranslation();
  const { data, refetch, isFetching } = useImageVersions();

  return (
    <>
      <Title mb={15}>{t("software_updates")}</Title>
      {data && (
        <Text fz="sm" mb={15}>
          {data.available
            ? t("new_version_available")
            : t("already_latest_version")}
        </Text>
      )}
      <Group>
        <Button
          type="button"
          loading={isFetching}
          disabled={isFetching}
          onClick={() => {
            refetch();
          }}
        >
          {t("check_for_updates")}
        </Button>
        {data?.releaseNotesUrl && (
          <Anchor href={data.releaseNotesUrl} target="_blank">
            <Group align="center">
              {t("release_notes")}
              <IconExternalLink />
            </Group>
          </Anchor>
        )}
      </Group>
    </>
  );
};

export default Updates;
