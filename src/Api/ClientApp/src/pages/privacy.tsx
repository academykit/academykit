import { ReactMarkdown } from "react-markdown/lib/react-markdown";
import { Container } from "@mantine/core";

export const PrivacyPage = () => {
  const markDownContent = `
  
  # Privacy Policy

## Overview

Welcome to Vurilo.com, a platform to learn; to teach.

Vurilo application and the respective website www.vurilo.com are owned, operated and distributed by Vurilo.

By accessing any part of the Services, you are agreeing to the terms and conditions described below (this “Privacy Policy. If anyone do not agree to these terms, then one should not use this Services. This Privacy Policy applies to all kind of users, including the one who simply use this platform to view the content and to the one who has been a registered user of this platform.

We may, at our sole discretion, modify this Privacy Policy at any time. Use of information we collect now is subject to the Privacy Policy in effect at the time such information is used. If we make material changes in the way we collect or use information, we will notify you by posting an announcement on the Services or sending you an email. By accessing the Services at any time after such modifications, you are agreeing to be bound by the updated policy.

This Privacy Policy was last modified as of May 25, 2021.

## Personal Information

Once the user is register with Vurilo platform as a Teacher or as a Member, we will be asked to provide Personal Information. which can be used to contact or individually identify you. This includes, but is not limited to, your name, email address, mailing address, contact number, Citizenship number, your profile picture etc.
Transactions on Vurilo involving your billing and payment information are processed by Bank or Khalti payment gateway and are subject to their privacy policies. We do not store any of the transaction detail that you provide to these payment processors.
In order to send you email from Vurilo, we need your email address. To provide you with additional services, we may also request your name, general location, and telephone number. We may give some or all of this information to the teachers for classes that you sign up for so that they can coordinate their classes. We will not give your name, email, or other Personal Data to unaffiliated third parties except with your consent or as otherwise set forth in this privacy policy. Though we employ industry standard measures to preserve user privacy, we may need to disclose Personal Data when required by law or legal process that we receive or to protect our interests or the safety of others. We may use your Personal Data to operate, improve, understand and personalize our Services. Information what we collect from Teachers are:
· First and last name
· Mailing address
· Contact Number
· Email address
· Date of birth
· Gender
· Citizenship Number
· Academic History
· Employment history
· Links to your social media accounts
· Any other information you choose to share publicly on the Services.

## Sharing Your Personal Information

We share your Personal Information with third parties to help us use your Personal Information, as described above.

We use Google Analytics to help us understand how our customers use the Site. You can read more about how Google uses your Personal Information here: https://www.google.com/intl/en/policies/privacy/.

You can also opt-out of Google Analytics here: https://tools.google.com/dlpage/gaoptout.

Finally, we may also share your Personal Information to comply with applicable laws and regulations, to respond to a subpoena, search warrant or other lawful request for information we receive, or to otherwise protect our rights.

### Do Not Track

Please note that we do not alter our Site’s data collection and use practices when we see a Do Not Track signal from your browser.

## Minors

The Site is not intended for individuals under the age of 18. However, the members of a team can be below 18.

## Disclosure

Any Personal Data or content that you voluntarily disclose in public areas becomes publicly available and can be collected and used by other users for any reason.
Your profile pages (“Profile”) and any online classrooms you participate in are typically publicly available.
You have the ability to change your account settings to limit access to your profile, and may delete content you post to public classrooms.
If you need assistance with controlling your account privacy settings or with deleting content, please contact us at support@vurilo.com.
Removal of posted content does not ensure complete or comprehensive removal of the content or information (e.g., some content may be “invisible” to other users but remain on our servers in some form).
You should exercise caution before disclosing your Personal Data via these or any other public venues.

## Changes

We may update this privacy policy from time to time in order to reflect, for example, changes to our practices or for other operational, legal or regulatory reasons.

## Contact Us

For more information about our privacy practices, if you have questions, or if you would like to make a complaint, please contact us by e-mail at support@vurilo.com

  
  `;

  return (
    <Container>
      <ReactMarkdown children={markDownContent} />
    </Container>
  );
};

export default PrivacyPage;
